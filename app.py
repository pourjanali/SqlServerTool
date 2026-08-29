import sys
import os
import re
from dataclasses import dataclass, field
from enum import Enum
import concurrent.futures
import wmi
import pyodbc
import pythoncom
from datetime import datetime

APP_VERSION = "2.0.0" # Updated version indicating the Python rewrite

# Developer-only startup option. Set to False for a local run or development
# build when the password prompt should be skipped.
REQUIRE_ACCESS_PASSWORD = False

# PyQt6 imports for GUI
from PyQt6.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout, QGridLayout,
    QGroupBox, QLabel, QLineEdit, QRadioButton, QPushButton, QTextEdit,
    QProgressBar, QTabWidget, QSplitter, QListWidget, QScrollArea, QComboBox,
    QTableWidget, QTableWidgetItem, QFileDialog, QMessageBox, QInputDialog,
    QSizePolicy, QHeaderView, QAbstractItemView
)
from PyQt6.QtCore import Qt, QThread, pyqtSignal, QTimer
from PyQt6.QtGui import QFont, QColor, QPalette, QCursor, QIcon


class DatabaseType(Enum):
    UNKNOWN = "Unknown"
    SEPIDAR = "Sepidar"
    DASHT = "Dasht"


def get_current_time_password(now: datetime | None = None) -> str:
    """Return the local-time access code, such as 424 for 04:24 PM."""
    now = now or datetime.now()
    return f"{int(now.strftime('%I'))}{now.strftime('%M')}"


def request_time_password() -> bool:
    """Prompt until the current local-time access code is entered or cancelled."""
    while True:
        password, accepted = QInputDialog.getText(
            None,
            "Security Check",
            "Enter the access password:",
            QLineEdit.EchoMode.Password,
        )
        if not accepted:
            return False
        if password == get_current_time_password():
            return True
        QMessageBox.warning(
            None,
            "Incorrect Password",
            "The password is incorrect.",
        )


@dataclass
class DbDetails:
    data_version: str = "N/A"
    company_name: str = "N/A"
    db_type: DatabaseType = DatabaseType.UNKNOWN
    fiscal_years: list[str] = field(default_factory=list)
    activation_code: str = "N/A"
    user_access_mode: str = "N/A"
    tran_count: str = "N/A"
    connections: str = "N/A"
    server_name: str = "N/A"
    service_name: str = "N/A"
    language: str = "N/A"
    collation: str = "N/A"

@dataclass
class TriggerInfo:
    name: str
    table: str
    event: str
    status: str

    def __str__(self):
        return f"'{self.name}' on table '{self.table}' (Event: {self.event}, Status: {self.status})"

@dataclass
class DatabaseInfo:
    name: str = "N/A"
    mdf_path: str = "N/A"
    ldf_path: str = "N/A"
    size_mb: float = 0.0
    customer_code: str = ""
    special_category: str = ""
    mdf_drive_model: str = ""
    ldf_drive_model: str = ""

    @property
    def mdf_drive(self) -> str:
        if self.mdf_path and os.path.isabs(self.mdf_path):
            return os.path.splitdrive(self.mdf_path)[0]
        return ""

    @property
    def ldf_drive(self) -> str:
        if self.ldf_path and os.path.isabs(self.ldf_path):
            return os.path.splitdrive(self.ldf_path)[0]
        return ""

    def __str__(self):
        return self.name


class SystemInfoHelper:
    _drive_model_cache = {}

    @staticmethod
    def get_client_system_info() -> str:
        """Queries local WMI for CPU, RAM, GPU, and OS architecture."""
        pythoncom.CoInitialize()  # Required for WMI in threads
        try:
            c = wmi.WMI()
            sb = []

            # CPU
            try:
                cpus = c.Win32_Processor()
                sb.append(f"CPU: {cpus[0].Name if cpus else 'N/A'}")
            except Exception:
                sb.append("CPU: N/A")

            # RAM
            try:
                systems = c.Win32_ComputerSystem()
                if systems and systems[0].TotalPhysicalMemory:
                    ram_gb = round(int(systems[0].TotalPhysicalMemory) / 1073741824.0, 2)
                    sb.append(f"Installed RAM: {ram_gb} GB")
                else:
                    sb.append("Installed RAM: N/A")
            except Exception:
                sb.append("Installed RAM: N/A")

            # GPU
            try:
                gpus = c.Win32_VideoController()
                gpu_names = [gpu.Name for gpu in gpus if gpu.Name]
                sb.append(f"Graphics Card(s): {', '.join(gpu_names) if gpu_names else 'N/A'}")
            except Exception:
                sb.append("Graphics Card(s): N/A")

            # Storage
            try:
                drives = c.Win32_DiskDrive()
                storage_details = []
                for drive in drives:
                    if drive.Size:
                        size_gb = round(int(drive.Size) / 1073741824.0, 2)
                        storage_details.append(f"{drive.Model} ({size_gb} GB)")
                sb.append(f"Storage: {' | '.join(storage_details) if storage_details else 'N/A'}")
            except Exception:
                sb.append("Storage: N/A")

            # OS System Type
            try:
                os_info = c.Win32_OperatingSystem()
                if os_info:
                    sb.append(f"System Type: {os_info[0].OSArchitecture}")
                    sb.append(f"OS: {os_info[0].Caption}")
                else:
                    sb.append("System Type: N/A\nOS: N/A")
            except Exception:
                sb.append("System Type: N/A\nOS: N/A")

            return "\n".join(sb)
        finally:
            pythoncom.CoUninitialize()

    @staticmethod
    def get_drive_model(drive_letter: str) -> str:
        """Maps a logical drive letter (e.g. C) to the physical disk model."""
        if not drive_letter: return "N/A"
        drive_letter = drive_letter[0].upper()
        if drive_letter in SystemInfoHelper._drive_model_cache:
            return SystemInfoHelper._drive_model_cache[drive_letter]

        pythoncom.CoInitialize()
        try:
            c = wmi.WMI()
            model = "N/A"
            # Win32_LogicalDisk -> Win32_DiskPartition -> Win32_DiskDrive
            logical_disks = c.query(f"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{drive_letter}:'}} WHERE AssocClass=Win32_LogicalDiskToPartition")
            if logical_disks:
                partition_id = logical_disks[0].DeviceID
                disk_drives = c.query(f"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition_id}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition")
                if disk_drives:
                    model = disk_drives[0].Model or "N/A"

            SystemInfoHelper._drive_model_cache[drive_letter] = model
            return model
        except Exception:
            return "N/A"
        finally:
            pythoncom.CoUninitialize()


class SqlManager:
    """Manages all interactions with SQL Server via pyodbc."""

    def __init__(self, connection_string: str):
        if not connection_string:
            raise ValueError("Connection string cannot be empty")
        self._connection_string = connection_string

    def _get_connection(self, db_name="master", autocommit=False):
        """Creates and returns a new pyodbc connection."""
        # Replace the initial database in the connection string
        conn_str = re.sub(r"Database=.*?;", f"Database={db_name};", self._connection_string, flags=re.IGNORECASE)
        try:
            # We enforce autocommit based on the parameter (needed for BACKUP/RESTORE)
            return pyodbc.connect(conn_str, autocommit=autocommit)
        except pyodbc.Error as e:
            raise Exception(f"Database connection error: {str(e)}")

    def execute_non_query(self, command_text: str, db_name: str, use_master: bool = False, progress_callback=None):
        target_db = "master" if use_master else db_name

        needs_autocommit = False
        upper_cmd = command_text.upper().strip()
        if upper_cmd.startswith("BACKUP") or upper_cmd.startswith("RESTORE") or upper_cmd.startswith("CREATE") or upper_cmd.startswith("DROP") or upper_cmd.startswith("DBCC"):
            needs_autocommit = True

        needs_single_user = upper_cmd.startswith("RESTORE") and not upper_cmd.startswith("RESTORE VERIFYONLY")

        # Set Single User Mode if needed (like the C# behavior)
        if needs_single_user:
            try:
                with self._get_connection("master", autocommit=True) as master_conn:
                    with master_conn.cursor() as cursor:
                        cursor.execute(f"ALTER DATABASE [{db_name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE")
            except Exception as e:
                print(f"Warning: Could not set SINGLE_USER: {e}")

        # Execute primary command
        try:
            with self._get_connection(target_db, autocommit=needs_autocommit) as conn:
                with conn.cursor() as cursor:
                    cursor.execute(command_text)
                    # Loop through result sets to catch messages (pyodbc workaround for InfoMessage)
                    while cursor.nextset():
                        pass
        except pyodbc.Error as e:
            # Re-raise unless it's just informational (pyodbc sometimes throws errors for info)
            raise Exception(f"SQL Execution Error: {str(e)}")

        # Set Multi User Mode after restore
        if needs_single_user:
            try:
                with self._get_connection("master", autocommit=True) as master_conn:
                    with master_conn.cursor() as cursor:
                        cursor.execute(f"ALTER DATABASE [{db_name}] SET MULTI_USER")
            except Exception as e:
                print(f"Warning: Could not set MULTI_USER: {e}")

    def get_server_system_info(self) -> tuple[str, int, int]:
        """Returns (Version, RAM_MB, CPU_Count). Replicates C# modern/legacy logic."""
        version = "N/A"
        memory = 0
        cpus = 0

        with self._get_connection("master") as conn:
            with conn.cursor() as cursor:
                cursor.execute("SELECT @@VERSION")
                row = cursor.fetchone()
                if row:
                    version = str(row[0])

                is_legacy = any(x in version for x in ["2008", "2005", "2000"])

                if is_legacy:
                    try:
                        cursor.execute("EXEC sp_readerrorlog 0, 1, 'detected'")
                        rows = cursor.fetchall()
                        for r in rows:
                            log_text = str(r[2]) if len(r) > 2 else ""
                            if "detected" in log_text and "MB of RAM" in log_text:
                                match = re.search(r"(\d+)\s*MB of RAM", log_text)
                                if match:
                                    memory = int(match.group(1))
                                    break

                        cursor.execute("SELECT cpu_count FROM sys.dm_os_sys_info")
                        cpu_row = cursor.fetchone()
                        if cpu_row: cpus = int(cpu_row[0])
                    except Exception:
                        pass
                else:
                    # Modern SQL Server
                    cursor.execute("SELECT cpu_count, physical_memory_kb FROM sys.dm_os_sys_info")
                    row = cursor.fetchone()
                    if row:
                        cpus = int(row[0])
                        memory = int(row[1]) // 1024

        return version, memory, cpus

    def get_sql_short_version(self) -> str:
        with self._get_connection("master") as conn:
            with conn.cursor() as cursor:
                cursor.execute("SELECT @@VERSION")
                version = str(cursor.fetchone()[0])

        if "2022" in version: return "SQL2022"
        if "2019" in version: return "SQL2019"
        if "2017" in version: return "SQL2017"
        if "2016" in version: return "SQL2016"
        if "2014" in version: return "SQL2014"
        if "2012" in version: return "SQL2012"
        if "2008 R2" in version: return "SQL2008R2"
        if "2008" in version: return "SQL2008"
        if "2005" in version: return "SQL2005"
        return "SQL"

    def get_default_data_path(self) -> str:
        try:
            with self._get_connection("master") as conn:
                with conn.cursor() as cursor:
                    cursor.execute("SELECT SERVERPROPERTY('InstanceDefaultDataPath');")
                    row = cursor.fetchone()
                    if row and row[0]:
                        return str(row[0])

                    cursor.execute("SELECT physical_name FROM sys.master_files WHERE database_id = 1 AND file_id = 1;")
                    row = cursor.fetchone()
                    if row and row[0]:
                        return os.path.dirname(str(row[0]))
        except Exception:
            return ""
        return ""

    def get_databases_and_files(self) -> list[DatabaseInfo]:
        databases = {}
        properties_query = """
            SET NOCOUNT ON;

            IF OBJECT_ID('tempdb..#DatabaseProperties') IS NOT NULL
                DROP TABLE #DatabaseProperties;

            CREATE TABLE #DatabaseProperties (
                DatabaseName sysname NOT NULL PRIMARY KEY,
                CustomerCode nvarchar(4000) NULL,
                SpecialCategory nvarchar(4000) NULL
            );

            DECLARE @database_name sysname;
            DECLARE @statement nvarchar(max);

            DECLARE database_cursor CURSOR LOCAL FAST_FORWARD FOR
                SELECT name
                FROM sys.databases
                WHERE database_id > 4 AND state = 0;

            OPEN database_cursor;
            FETCH NEXT FROM database_cursor INTO @database_name;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY
                    SET @statement = N'
                        USE ' + QUOTENAME(@database_name) + N';
                        INSERT INTO #DatabaseProperties
                            (DatabaseName, CustomerCode, SpecialCategory)
                        SELECT
                            @property_database_name,
                            MAX(CASE WHEN name = N''CustomerCode''
                                     THEN CONVERT(nvarchar(4000), value) END),
                            MAX(CASE WHEN name = N''IsSGDB''
                                     THEN CONVERT(nvarchar(4000), value) END)
                        FROM sys.extended_properties
                        WHERE class = 0
                          AND name IN (N''CustomerCode'', N''IsSGDB'');';

                    EXEC sys.sp_executesql
                        @statement,
                        N'@property_database_name sysname',
                        @property_database_name = @database_name;
                END TRY
                BEGIN CATCH
                    -- Some databases may be inaccessible; retain them with blank optional properties.
                END CATCH;

                FETCH NEXT FROM database_cursor INTO @database_name;
            END;

            CLOSE database_cursor;
            DEALLOCATE database_cursor;

            SELECT DatabaseName, CustomerCode, SpecialCategory
            FROM #DatabaseProperties;
        """
        query = """
            SELECT d.name, mf.physical_name, mf.type_desc, CAST(mf.size AS BIGINT) * 8 AS FileSizeKb
            FROM sys.databases d
            JOIN sys.master_files mf ON d.database_id = mf.database_id
            WHERE d.database_id > 4
            ORDER BY d.name;
        """
        with self._get_connection("master") as conn:
            with conn.cursor() as cursor:
                properties = {}
                cursor.execute(properties_query)

                # The batch contains setup statements before the final SELECT result set.
                while cursor.description is None and cursor.nextset():
                    pass

                if cursor.description:
                    for row in cursor.fetchall():
                        properties[str(row[0])] = (
                            str(row[1]) if row[1] is not None else "",
                            str(row[2]) if row[2] is not None else "",
                        )

                cursor.execute(query)
                for row in cursor.fetchall():
                    db_name = row[0]
                    file_path = row[1]
                    file_type = row[2]
                    file_size_mb = int(row[3]) / 1024.0

                    if db_name not in databases:
                        databases[db_name] = DatabaseInfo(name=db_name)

                    db_info = databases[db_name]
                    db_info.customer_code, db_info.special_category = properties.get(
                        str(db_name), ("", "")
                    )
                    if file_type == "ROWS":
                        db_info.mdf_path = file_path
                    elif file_type == "LOG":
                        db_info.ldf_path = file_path

                    db_info.size_mb += file_size_mb

        return sorted(list(databases.values()), key=lambda x: x.name)

    def backup_database(self, db_name: str, backup_path: str, verify: bool, progress_callback=None) -> bool:
        if progress_callback: progress_callback(5)
        cmd = f"BACKUP DATABASE [{db_name}] TO DISK = N'{backup_path}' WITH CHECKSUM, NOFORMAT, INIT, NAME = N'{db_name}-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
        self.execute_non_query(cmd, db_name, use_master=True, progress_callback=progress_callback)

        if verify:
            if progress_callback: progress_callback(95)
            verify_cmd = f"RESTORE VERIFYONLY FROM DISK = N'{backup_path}' WITH CHECKSUM"
            self.execute_non_query(verify_cmd, db_name, use_master=True)

        if progress_callback: progress_callback(100)
        return True

    def delete_database(self, db_name: str) -> bool:
        with self._get_connection("master", autocommit=True) as conn:
            with conn.cursor() as cursor:
                try:
                    # SINGLE_USER mode to forcefully drop other active connections
                    cursor.execute(f"ALTER DATABASE [{db_name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE")
                except pyodbc.Error as e:
                    # If setting SINGLE_USER fails (e.g., due to permissions), DROP DATABASE will hang indefinitely.
                    # We must abort and alert the user.
                    raise Exception(f"Could not gain exclusive access to delete '{db_name}'. You may lack permissions.\nDetails: {str(e)}")

                # Drop database
                try:
                    cursor.execute(f"DROP DATABASE [{db_name}]")
                except pyodbc.Error as e:
                    raise Exception(f"Failed to drop database '{db_name}'.\nDetails: {str(e)}")

        return True

    def attach_database(self, db_name: str, mdf_path: str, ldf_path: str) -> bool:
        if ldf_path and os.path.exists(ldf_path):
            cmd = f"CREATE DATABASE [{db_name}] ON (FILENAME = N'{mdf_path}'), (FILENAME = N'{ldf_path}') FOR ATTACH"
        else:
            cmd = f"CREATE DATABASE [{db_name}] ON (FILENAME = N'{mdf_path}') FOR ATTACH_REBUILD_LOG"

        self.execute_non_query(cmd, db_name, use_master=True)
        return True

    def check_database(self, db_name: str) -> tuple[str, bool]:
        output = []
        has_errors = False
        try:
            # DBCC operations require autocommit
            with self._get_connection(db_name, autocommit=True) as conn:
                with conn.cursor() as cursor:
                    cursor.execute(f"DBCC CHECKDB ('{db_name}') WITH ALL_ERRORMSGS, NO_INFOMSGS")
                    while cursor.nextset():
                        pass

            output_str = f"DBCC CHECKDB for '{db_name}' completed without errors."
        except pyodbc.Error as e:
            output_str = str(e)
            has_errors = True

        return output_str, has_errors

    def detach_database(self, db_name: str) -> bool:
        with self._get_connection("master", autocommit=True) as conn:
            with conn.cursor() as cursor:
                try:
                    cursor.execute(f"ALTER DATABASE [{db_name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE")
                    cursor.execute(f"EXEC sp_detach_db '{db_name}', 'true'")
                except Exception as e:
                    try:
                        cursor.execute(f"ALTER DATABASE [{db_name}] SET MULTI_USER")
                    except Exception: pass
                    raise e
        return True

    def restore_database(self, db_name: str, backup_path: str, new_mdf: str, new_ldf: str, progress_callback=None) -> bool:
        if progress_callback: progress_callback(5)
        file_list = self.get_backup_file_list(backup_path)
        if not file_list or len(file_list) < 2:
            raise Exception("Could not read the logical file names from the backup set.")

        mdf_logical = next((f[0] for f in file_list if f[1] == "D"), None)
        ldf_logical = next((f[0] for f in file_list if f[1] == "L"), None)

        cmd = f"""
            RESTORE DATABASE [{db_name}]
            FROM DISK = N'{backup_path}'
            WITH MOVE N'{mdf_logical}' TO N'{new_mdf}',
                 MOVE N'{ldf_logical}' TO N'{new_ldf}',
                 REPLACE, STATS = 10
        """
        self.execute_non_query(cmd, db_name, use_master=True, progress_callback=progress_callback)
        if progress_callback: progress_callback(100)
        return True

    def get_original_database_name(self, backup_path: str) -> str:
        cmd = f"RESTORE HEADERONLY FROM DISK = N'{backup_path}'"
        with self._get_connection("master") as conn:
            with conn.cursor() as cursor:
                cursor.execute(cmd)
                columns = [column[0] for column in cursor.description]
                if 'DatabaseName' in columns:
                    idx = columns.index('DatabaseName')
                    row = cursor.fetchone()
                    if row: return row[idx]
        return ""

    def get_backup_file_list(self, backup_path: str) -> list[tuple[str, str]]:
        files = []
        cmd = f"RESTORE FILELISTONLY FROM DISK = N'{backup_path}'"
        with self._get_connection("master") as conn:
            with conn.cursor() as cursor:
                cursor.execute(cmd)
                columns = [c[0] for c in cursor.description]
                logical_idx = columns.index('LogicalName')
                type_idx = columns.index('Type')
                for row in cursor.fetchall():
                    files.append((row[logical_idx], row[type_idx]))
        return files

    def get_database_details(self, db_name: str) -> DbDetails:
        details = DbDetails()

        try:
            details.user_access_mode = self.get_user_access_mode(db_name)
        except Exception:
            details.user_access_mode = "Error"

        try:
            with self._get_connection(db_name) as conn:
                with conn.cursor() as cursor:
                    # Server info
                    server_query = f"""
                        SELECT @@TRANCOUNT AS TranCount,
                               (SELECT COUNT(*) FROM sys.dm_exec_connections) AS Connections,
                               @@SERVERNAME AS ServerName,
                               @@SERVICENAME AS ServiceName,
                               @@LANGUAGE AS Language,
                               DATABASEPROPERTYEX('{db_name}', 'Collation') AS Collation;
                    """
                    cursor.execute(server_query)
                    row = cursor.fetchone()
                    if row:
                        details.tran_count = str(row[0])
                        details.connections = str(row[1])
                        details.server_name = str(row[2])
                        details.service_name = str(row[3])
                        details.language = str(row[4])
                        details.collation = str(row[5])

                    # Data Version
                    try:
                        cursor.execute("SELECT TOP 1 CAST(Major AS VARCHAR) + '.' + CAST(Minor AS VARCHAR) + '.' + CAST(Build AS VARCHAR) FROM fmk.Version ORDER BY VersionID DESC")
                        row = cursor.fetchone()
                        if row: details.data_version = str(row[0])
                    except Exception: pass

                    # Company Name
                    try:
                        cursor.execute("SELECT Value FROM fmk.Configuration WHERE [Key] = N'CompanyName'")
                        row = cursor.fetchone()
                        if row: details.company_name = str(row[0])
                    except Exception: pass

                    # Activation Code
                    try:
                        cursor.execute("SELECT value FROM sys.extended_properties where name = 'ActivationCode'")
                        row = cursor.fetchone()
                        if row: details.activation_code = str(row[0])
                    except Exception: pass

                    # Determine DB Type and Fiscal Years
                    try:
                        cursor.execute("SELECT Title FROM fmk.fiscalyear")
                        for r in cursor.fetchall():
                            details.fiscal_years.append(str(r[0]))
                        details.db_type = DatabaseType.SEPIDAR
                    except pyodbc.Error:
                        try:
                            cursor.execute("SELECT Title FROM fmk.fiscalperiod")
                            for r in cursor.fetchall():
                                details.fiscal_years.append(str(r[0]))
                            details.db_type = DatabaseType.DASHT
                        except pyodbc.Error:
                            details.db_type = DatabaseType.UNKNOWN

        except Exception:
            pass # Gracefully degrade on failure

        return details

    def get_user_access_mode(self, db_name: str) -> str:
        with self._get_connection("master") as conn:
            with conn.cursor() as cursor:
                cursor.execute(f"SELECT DATABASEPROPERTYEX('{db_name}', 'UserAccess')")
                row = cursor.fetchone()
                return str(row[0]) if row else "N/A"

    def check_database_schema(self, db_name: str) -> tuple[bool, str]:
        details = self.get_database_details(db_name)
        if details.db_type == DatabaseType.UNKNOWN:
            return False, f"Could not determine the database type (Sepidar/Dasht) for '{db_name}'."

        dasht_schemas = {"ACC", "FMK", "GNR", "JWL", "MSG", "POS", "PROP", "SCD"}
        sepidar_schemas = {"ACC", "AST", "CNT", "DST", "FMK", "GNR", "INV", "MRP", "MSG", "PAY", "POM", "POS", "RPA", "SCD", "SLS", "WKO"}
        expected_schemas = dasht_schemas if details.db_type == DatabaseType.DASHT else sepidar_schemas

        actual_schemas = set()
        query = """
            SELECT t.TABLE_SCHEMA
            FROM INFORMATION_SCHEMA.TABLES AS t
            WHERE t.TABLE_TYPE = 'BASE TABLE'
            GROUP BY t.TABLE_SCHEMA
            ORDER BY t.TABLE_SCHEMA;
        """
        with self._get_connection(db_name) as conn:
            with conn.cursor() as cursor:
                cursor.execute(query)
                for row in cursor.fetchall():
                    actual_schemas.add(row[0].upper())

        actual_schemas.discard("INFORMATION_SCHEMA")
        actual_schemas.discard("SYS")
        actual_schemas.discard("DBO")

        missing = expected_schemas - actual_schemas
        extra = actual_schemas - expected_schemas

        if expected_schemas.issubset(actual_schemas) and not extra:
            return True, f"Schema for '{db_name}' ({details.db_type.value}) is correct.\nAll {len(expected_schemas)} expected schemas were found."
        else:
            msg = [f"Schema mismatch found for '{db_name}' ({details.db_type.value}):"]
            if missing: msg.append(f"- Missing schemas: {', '.join(missing)}")
            if extra: msg.append(f"- Unexpected extra schemas: {', '.join(extra)}")
            return False, "\n".join(msg)

    def check_database_triggers(self, db_name: str) -> tuple[bool, str]:
        details = self.get_database_details(db_name)
        if details.db_type == DatabaseType.UNKNOWN:
            return False, f"Could not determine the database type (Sepidar/Dasht) for '{db_name}'."

        found_triggers = []
        query = """
            SELECT
                trg.name AS trigger_name,
                ISNULL(SCHEMA_NAME(tab.schema_id) + '.' + tab.name, 'DATABASE') AS [table],
                (CASE WHEN OBJECTPROPERTY(trg.object_id, 'ExecIsUpdateTrigger') = 1 THEN 'Update ' ELSE '' END +
                 CASE WHEN OBJECTPROPERTY(trg.object_id, 'ExecIsDeleteTrigger') = 1 THEN 'Delete ' ELSE '' END +
                 CASE WHEN OBJECTPROPERTY(trg.object_id, 'ExecIsInsertTrigger') = 1 THEN 'Insert' ELSE '' END
                ) AS [event],
                CASE WHEN is_disabled = 1 THEN 'Disabled' ELSE 'Active' END AS [status]
            FROM sys.triggers trg
            LEFT JOIN sys.objects tab ON trg.parent_id = tab.object_id
            WHERE trg.is_ms_shipped = 0
            ORDER BY trg.name;
        """
        with self._get_connection(db_name) as conn:
            with conn.cursor() as cursor:
                cursor.execute(query)
                for row in cursor.fetchall():
                    found_triggers.append(TriggerInfo(
                        name=row[0], table=row[1], event=row[2].strip() if row[2] else "", status=row[3]
                    ))

        if details.db_type == DatabaseType.SEPIDAR:
            if found_triggers:
                msg = [f"RED FLAG: Sepidar database '{db_name}' should not have any triggers, but {len(found_triggers)} were found:"]
                for t in found_triggers: msg.append(f"- {t}")
                return False, "\n".join(msg)
            else:
                return True, f"SUCCESS: No triggers found in Sepidar database '{db_name}', as expected."
        else:
            allowed = {"ItemChangeVersion", "ItemGroupChangeVersion", "ItemGroupItemChangeVersion",
                       "ItemSalePriceChangeVersion", "ItemSubUnitChangeVersion", "PartyChangeVersion", "UserChangeVersion"}
            unexpected = [t for t in found_triggers if t.name not in allowed]
            if unexpected:
                msg = [f"WARNING: Found {len(unexpected)} unexpected trigger(s) in Dasht database '{db_name}':"]
                for t in unexpected: msg.append(f"- {t}")
                return False, "\n".join(msg)
            else:
                return True, f"SUCCESS: All {len(found_triggers)} triggers found in Dasht database '{db_name}' are standard."

    def execute_query(self, query: str, db_name: str) -> tuple[list, list]:
        """Executes a query and returns (columns, rows)."""
        with self._get_connection(db_name) as conn:
            with conn.cursor() as cursor:
                cursor.execute(query)
                if cursor.description:
                    columns = [desc[0] for desc in cursor.description]
                    rows = cursor.fetchall()
                    return columns, rows
                return [], []

class Worker(QThread):
    finished = pyqtSignal(object)
    error = pyqtSignal(str)
    progress = pyqtSignal(int)
    log = pyqtSignal(str)

    def __init__(self, fn, *args, **kwargs):
        super().__init__()
        self.fn = fn
        self.args = args
        self.kwargs = kwargs

    def run(self):
        try:
            # Inject callbacks if expected
            import inspect
            sig = inspect.signature(self.fn)
            if 'progress_callback' in sig.parameters:
                self.kwargs['progress_callback'] = self.progress.emit
            if 'log_callback' in sig.parameters:
                self.kwargs['log_callback'] = self.log.emit

            result = self.fn(*self.args, **self.kwargs)
            self.finished.emit(result)
        except Exception as e:
            self.error.emit(str(e))

class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle(f"SQL Server Utility v{APP_VERSION} (Python Edition)")
        self.resize(1100, 800)
        self.sql_manager = None
        self._databases: list[DatabaseInfo] = []
        self._sql_short_version = "SQL"

        self.setup_ui()
        self.update_auth_controls()
        self.set_disconnected_state()

        # Load system info asynchronously so GUI boots fast
        self.display_client_system_info_async()

    def setup_ui(self):
        central_widget = QWidget()
        self.setCentralWidget(central_widget)
        main_layout = QVBoxLayout(central_widget)

        # Top Panel
        top_panel = QWidget()
        top_layout = QHBoxLayout(top_panel)
        top_layout.setContentsMargins(0,0,0,0)

        # Connection GroupBox
        self.gb_connection = QGroupBox("Connection Settings")
        conn_layout = QGridLayout(self.gb_connection)

        self.rb_win_auth = QRadioButton("Windows Authentication")
        self.rb_sql_auth = QRadioButton("SQL Server Authentication")
        self.rb_sql_auth.setChecked(True)
        self.rb_win_auth.toggled.connect(self.update_auth_controls)
        self.rb_sql_auth.toggled.connect(self.update_auth_controls)

        self.txt_server = QLineEdit(".\\Sepidar")
        self.txt_user = QLineEdit("damavand")
        self.txt_password = QLineEdit("")
        self.txt_password.setEchoMode(QLineEdit.EchoMode.Password)

        self.btn_browse = QPushButton("...")
        self.btn_browse.setFixedWidth(40)
        self.btn_browse.clicked.connect(self.browse_servers)

        self.btn_connect = QPushButton("Connect")
        self.btn_connect.setStyleSheet("background-color: SeaGreen; color: white; font-weight: bold; padding: 15px;")
        self.btn_connect.clicked.connect(self.connect_to_server)

        self.btn_disconnect = QPushButton("Disconnect")
        self.btn_disconnect.setStyleSheet("background-color: IndianRed; color: white; font-weight: bold; padding: 15px;")
        self.btn_disconnect.setVisible(False)
        self.btn_disconnect.clicked.connect(self.set_disconnected_state)

        conn_layout.addWidget(self.rb_win_auth, 0, 0, 1, 2)
        conn_layout.addWidget(self.rb_sql_auth, 0, 2, 1, 2)
        conn_layout.addWidget(QLabel("Server:"), 1, 0)
        conn_layout.addWidget(self.txt_server, 1, 1, 1, 2)
        conn_layout.addWidget(self.btn_browse, 1, 3)
        conn_layout.addWidget(QLabel("User:"), 2, 0)
        conn_layout.addWidget(self.txt_user, 2, 1, 1, 3)
        conn_layout.addWidget(QLabel("Password:"), 3, 0)
        conn_layout.addWidget(self.txt_password, 3, 1, 1, 3)
        conn_layout.addWidget(self.btn_connect, 0, 4, 3, 1)
        conn_layout.addWidget(self.btn_disconnect, 0, 4, 3, 1)

        # System Info GroupBox
        self.gb_system = QGroupBox("System & Server Details")
        sys_layout = QVBoxLayout(self.gb_system)
        self.lbl_sql_version = QLabel("SQL Version: N/A")
        self.lbl_sql_version.setStyleSheet("font-weight: bold;")
        self.txt_system_details = QTextEdit("Client Hardware: Loading...")
        self.txt_system_details.setReadOnly(True)
        sys_layout.addWidget(self.lbl_sql_version)
        sys_layout.addWidget(self.txt_system_details)

        top_layout.addWidget(self.gb_connection, 1)
        top_layout.addWidget(self.gb_system, 1)
        main_layout.addWidget(top_panel, 0)

        # Tab Control
        self.tab_widget = QTabWidget()
        self.setup_db_manager_tab()
        self.setup_query_workbook_tab()
        main_layout.addWidget(self.tab_widget, 1)

        # Bottom Log and Progress
        bottom_panel = QWidget()
        bottom_layout = QVBoxLayout(bottom_panel)
        bottom_layout.setContentsMargins(0,0,0,0)

        self.rtb_log = QTextEdit()
        self.rtb_log.setReadOnly(True)
        self.rtb_log.setFixedHeight(100)
        self.rtb_log.setStyleSheet("font-family: Consolas;")

        self.progress_bar = QProgressBar()
        self.progress_bar.setVisible(False)
        self.progress_bar.setRange(0, 100)

        bottom_layout.addWidget(self.rtb_log)
        bottom_layout.addWidget(self.progress_bar)
        main_layout.addWidget(bottom_panel, 0)

    def setup_db_manager_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)
        splitter = QSplitter(Qt.Orientation.Horizontal)

        # Left side - Database List
        left_widget = QWidget()
        left_layout = QVBoxLayout(left_widget)
        left_layout.setContentsMargins(0,0,0,0)
        lbl_dbs = QLabel("Databases")
        lbl_dbs.setStyleSheet("background-color: Gainsboro; padding: 5px; font-weight: bold;")
        self.table_databases = QTableWidget(0, 1)
        self.table_databases.setHorizontalHeaderLabels(["Database"])
        self.table_databases.setEditTriggers(
            QAbstractItemView.EditTrigger.NoEditTriggers
        )
        self.table_databases.setSelectionBehavior(
            QAbstractItemView.SelectionBehavior.SelectRows
        )
        self.table_databases.setSelectionMode(
            QAbstractItemView.SelectionMode.SingleSelection
        )
        self.table_databases.setShowGrid(False)
        self.table_databases.verticalHeader().setVisible(False)
        self.table_databases.verticalHeader().setDefaultSectionSize(18)
        database_header = self.table_databases.horizontalHeader()
        database_header.setVisible(False)
        database_header.setSectionResizeMode(0, QHeaderView.ResizeMode.Stretch)
        self.table_databases.setStyleSheet(
            "QTableWidget::item { padding: 0 4px; border: none; }"
        )
        self.table_databases.itemSelectionChanged.connect(self.on_db_selected)
        left_layout.addWidget(lbl_dbs)
        left_layout.addWidget(self.table_databases)

        # Right side - Flow of GroupBoxes (Simulated with scroll area and VBox)
        scroll = QScrollArea()
        scroll.setWidgetResizable(True)
        right_widget = QWidget()
        self.right_layout = QVBoxLayout(right_widget)
        self.right_layout.setAlignment(Qt.AlignmentFlag.AlignTop)

        self.build_db_info_group()
        self.build_server_stats_group()
        self.build_file_paths_group()
        self.build_db_operations_group()
        self.build_attach_group()

        scroll.setWidget(right_widget)
        splitter.addWidget(left_widget)
        splitter.addWidget(scroll)
        splitter.setSizes([262, 800])
        layout.addWidget(splitter)
        self.tab_widget.addTab(tab, "Database Manager")

    def build_db_info_group(self):
        # Db Info Layout containing two side-by-side boxes (Details and Fiscal Years)
        container = QWidget()
        layout = QHBoxLayout(container)
        layout.setContentsMargins(0,0,0,0)

        # Details
        gb = QGroupBox("Selected Database Details")
        gl = QGridLayout(gb)
        self.lbl_db_version = QLabel("Data Version: N/A")
        self.lbl_db_size = QLabel("Size: N/A")
        self.lbl_customer_code = QLabel("Customer Code: N/A")
        self.lbl_special_category = QLabel("Special Category: N/A")
        self.lbl_company = QLabel("Company: N/A")
        self.lbl_db_type = QLabel("Type: N/A")
        self.lbl_activation = QLabel("Activation Code: N/A")
        self.lbl_access_mode = QLabel("Access Mode: N/A")
        self.lbl_access_mode.setStyleSheet("font-weight: bold;")

        gl.addWidget(self.lbl_db_version, 0, 0)
        gl.addWidget(self.lbl_db_size, 0, 1)
        gl.addWidget(self.lbl_company, 1, 0)
        gl.addWidget(self.lbl_customer_code, 1, 1)
        gl.addWidget(self.lbl_db_type, 2, 0)
        gl.addWidget(self.lbl_special_category, 2, 1)
        gl.addWidget(self.lbl_activation, 3, 0)
        gl.addWidget(self.lbl_access_mode, 4, 0)

        # Fiscal Years
        gb_fiscal = QGroupBox("Fiscal Years")
        fl = QVBoxLayout(gb_fiscal)
        self.list_fiscal = QListWidget()
        fl.addWidget(self.list_fiscal)

        layout.addWidget(gb, 2)
        layout.addWidget(gb_fiscal, 1)
        self.right_layout.addWidget(container)

    def build_server_stats_group(self):
        gb = QGroupBox("Server & Connection Stats")
        gl = QGridLayout(gb)
        self.lbl_server_name = QLabel("Server Name: N/A")
        self.lbl_service_name = QLabel("Service Name: N/A")
        self.lbl_connections = QLabel("Connections: N/A")
        self.lbl_tran_count = QLabel("Tran Count: N/A")
        self.lbl_language = QLabel("Language: N/A")
        self.lbl_collation = QLabel("Collation: N/A")

        gl.addWidget(self.lbl_server_name, 0, 0)
        gl.addWidget(self.lbl_service_name, 1, 0)
        gl.addWidget(self.lbl_connections, 2, 0)
        gl.addWidget(self.lbl_tran_count, 0, 1)
        gl.addWidget(self.lbl_language, 1, 1)
        gl.addWidget(self.lbl_collation, 2, 1)
        self.right_layout.addWidget(gb)

    def build_file_paths_group(self):
        gb = QGroupBox("Physical File Locations")
        gl = QGridLayout(gb)
        self.txt_mdf_path = QLineEdit(); self.txt_mdf_path.setReadOnly(True)
        self.txt_ldf_path = QLineEdit(); self.txt_ldf_path.setReadOnly(True)
        self.lbl_mdf_drive = QLabel("[]")
        self.lbl_ldf_drive = QLabel("[]")
        btn_open_dir = QPushButton("Open Folder")
        btn_open_dir.clicked.connect(self.open_mdf_directory)

        gl.addWidget(QLabel("MDF Path:"), 0, 0)
        gl.addWidget(self.txt_mdf_path, 0, 1)
        gl.addWidget(self.lbl_mdf_drive, 0, 2)
        gl.addWidget(btn_open_dir, 0, 3, 2, 1)

        gl.addWidget(QLabel("LDF Path:"), 1, 0)
        gl.addWidget(self.txt_ldf_path, 1, 1)
        gl.addWidget(self.lbl_ldf_drive, 1, 2)
        self.right_layout.addWidget(gb)

    def build_db_operations_group(self):
        gb = QGroupBox("Database Operations")
        gl = QGridLayout(gb)

        btn_backup = QPushButton("Backup")
        btn_backup_verify = QPushButton("Backup + Verify")
        btn_detach = QPushButton("Detach DB")
        btn_delete = QPushButton("Delete DB")
        btn_delete.setStyleSheet("color: crimson; font-weight:bold;")
        btn_health = QPushButton("Check Health")
        btn_schema = QPushButton("Check Schema")
        btn_triggers = QPushButton("Check Triggers")
        btn_restore = QPushButton("Restore DB")

        btn_backup.clicked.connect(lambda: self.perform_backup(False))
        btn_backup_verify.clicked.connect(lambda: self.perform_backup(True))
        btn_delete.clicked.connect(self.perform_delete)
        btn_health.clicked.connect(self.perform_health_check)
        btn_schema.clicked.connect(self.perform_schema_check)
        btn_triggers.clicked.connect(self.perform_trigger_check)
        btn_detach.clicked.connect(self.perform_detach)
        btn_restore.clicked.connect(self.perform_restore)

        gl.addWidget(btn_backup, 0, 0)
        gl.addWidget(btn_backup_verify, 0, 1)
        gl.addWidget(btn_detach, 0, 2)
        gl.addWidget(btn_delete, 0, 3)
        gl.addWidget(btn_health, 1, 0)
        gl.addWidget(btn_schema, 1, 1)
        gl.addWidget(btn_restore, 1, 2)
        gl.addWidget(btn_triggers, 1, 3)
        self.right_layout.addWidget(gb)

    def build_attach_group(self):
        gb = QGroupBox("Attach New Database")
        gl = QGridLayout(gb)
        self.txt_attach_mdf = QLineEdit()
        self.txt_attach_ldf = QLineEdit()

        btn_browse_mdf = QPushButton("...")
        btn_browse_mdf.clicked.connect(lambda: self.browse_file(self.txt_attach_mdf, "MDF File (*.mdf)"))
        btn_browse_ldf = QPushButton("...")
        btn_browse_ldf.clicked.connect(lambda: self.browse_file(self.txt_attach_ldf, "LDF File (*.ldf)"))

        btn_attach = QPushButton("Attach")
        btn_attach.setStyleSheet("background-color: SeaGreen; color: white;")
        btn_attach.clicked.connect(self.perform_attach)

        gl.addWidget(QLabel("MDF:"), 0, 0)
        gl.addWidget(self.txt_attach_mdf, 0, 1)
        gl.addWidget(btn_browse_mdf, 0, 2)
        gl.addWidget(QLabel("LDF:"), 0, 3)
        gl.addWidget(self.txt_attach_ldf, 0, 4)
        gl.addWidget(btn_browse_ldf, 0, 5)
        gl.addWidget(btn_attach, 0, 6)
        self.right_layout.addWidget(gb)

    def setup_query_workbook_tab(self):
        tab = QWidget()
        layout = QVBoxLayout(tab)
        splitter = QSplitter(Qt.Orientation.Vertical)

        # Top Panel - Query Editor
        top = QWidget()
        top_layout = QVBoxLayout(top)

        tools = QWidget()
        tools_layout = QHBoxLayout(tools)
        tools_layout.setContentsMargins(0,0,0,0)
        self.cmb_dbs_query = QComboBox()
        self.cmb_dbs_query.setMinimumWidth(250)
        btn_open = QPushButton("Open"); btn_open.clicked.connect(self.open_query)
        btn_save = QPushButton("Save"); btn_save.clicked.connect(self.save_query)
        btn_exec = QPushButton("Execute (F5)")
        btn_exec.setStyleSheet("background-color: LightGreen; font-weight: bold;")
        btn_exec.clicked.connect(self.execute_query)

        tools_layout.addWidget(QLabel("Run query against:"))
        tools_layout.addWidget(self.cmb_dbs_query)
        tools_layout.addStretch()
        tools_layout.addWidget(btn_open)
        tools_layout.addWidget(btn_save)
        tools_layout.addWidget(btn_exec)

        self.txt_query = QTextEdit()
        self.txt_query.setStyleSheet("font-family: Consolas; font-size: 10pt;")

        top_layout.addWidget(tools)
        top_layout.addWidget(self.txt_query)

        # Bottom Panel - Results Grid
        self.dgv_results = QTableWidget()
        self.dgv_results.setEditTriggers(QTableWidget.EditTrigger.NoEditTriggers)

        splitter.addWidget(top)
        splitter.addWidget(self.dgv_results)
        layout.addWidget(splitter)
        self.tab_widget.addTab(tab, "Query Workbook")

    def log(self, message: str):
        if not message: return
        timestamp = datetime.now().strftime("%H:%M:%S")
        self.rtb_log.append(f"[{timestamp}] {message}")

    def update_auth_controls(self):
        enabled = self.rb_sql_auth.isChecked()
        self.txt_user.setEnabled(enabled)
        self.txt_password.setEnabled(enabled)

    def show_progress(self, message: str):
        self.log(message)
        self.progress_bar.setValue(0)
        self.progress_bar.setVisible(True)
        self.setCursor(QCursor(Qt.CursorShape.WaitCursor))
        self.tab_widget.setEnabled(False)
        self.gb_connection.setEnabled(False)

    def hide_progress(self):
        self.progress_bar.setValue(0)
        self.progress_bar.setVisible(False)
        self.setCursor(QCursor(Qt.CursorShape.ArrowCursor))
        self.tab_widget.setEnabled(self.sql_manager is not None)
        self.gb_connection.setEnabled(True)

    def set_connected_state(self):
        self.txt_server.setEnabled(False)
        self.btn_browse.setEnabled(False)
        self.rb_win_auth.setEnabled(False)
        self.rb_sql_auth.setEnabled(False)
        self.txt_user.setEnabled(False)
        self.txt_password.setEnabled(False)

        self.btn_connect.setVisible(False)
        self.btn_disconnect.setVisible(True)
        self.tab_widget.setEnabled(True)
        self.tab_widget.setCurrentIndex(0)

    def set_disconnected_state(self):
        self.sql_manager = None

        self.txt_server.setEnabled(True)
        self.btn_browse.setEnabled(True)
        self.rb_win_auth.setEnabled(True)
        self.rb_sql_auth.setEnabled(True)
        self.update_auth_controls()

        self.btn_connect.setVisible(True)
        self.btn_disconnect.setVisible(False)

        self.lbl_sql_version.setText("SQL Version: N/A")

        self.table_databases.setRowCount(0)
        self.list_fiscal.clear()
        self.cmb_dbs_query.clear()

        self.txt_mdf_path.clear()
        self.txt_ldf_path.clear()
        self.lbl_mdf_drive.setText("")
        self.lbl_ldf_drive.setText("")

        self.lbl_db_version.setText("Data Version: N/A")
        self.lbl_db_size.setText("Size: N/A")
        self.lbl_customer_code.setText("Customer Code: N/A")
        self.lbl_special_category.setText("Special Category: N/A")
        self.lbl_company.setText("Company: N/A")
        self.lbl_db_type.setText("Type: N/A")
        self.lbl_activation.setText("Activation Code: N/A")
        self.lbl_access_mode.setText("Access Mode: N/A")
        self.lbl_access_mode.setStyleSheet("font-weight: bold; color: black;")

        self.lbl_server_name.setText("Server Name: N/A")
        self.lbl_service_name.setText("Service Name: N/A")
        self.lbl_connections.setText("Connections: N/A")
        self.lbl_tran_count.setText("Tran Count: N/A")
        self.lbl_language.setText("Language: N/A")
        self.lbl_collation.setText("Collation: N/A")

        self.dgv_results.clear()
        self.dgv_results.setRowCount(0)
        self.dgv_results.setColumnCount(0)

        self.tab_widget.setEnabled(False)
        self.hide_progress()
        self.log("Disconnected.")

    def display_client_system_info_async(self):
        self.sys_worker = Worker(SystemInfoHelper.get_client_system_info)
        self.sys_worker.finished.connect(self._on_client_sys_info_loaded)
        self.sys_worker.start()

    def _on_client_sys_info_loaded(self, info: str):
        self.client_info_cache = info
        # If server is connected, preserve server info
        if self.sql_manager:
            parts = self.txt_system_details.toPlainText().split("---\n")
            if len(parts) > 1:
                self.txt_system_details.setText(f"{parts[0]}---\n{info}")
            else:
                self.txt_system_details.setText(info)
        else:
            self.txt_system_details.setText(info)

    def connect_to_server(self):
        server = self.txt_server.text().strip()
        win_auth = self.rb_win_auth.isChecked()
        user = self.txt_user.text().strip()
        pwd = self.txt_password.text()

        # Build pyodbc connection string. Prefer ODBC Driver 17, fallback to SQL Server if missing.
        driver = "{ODBC Driver 17 for SQL Server}"
        # For broader compatibility, standard {SQL Server} driver is acceptable in older systems but lacks some features.

        if win_auth:
            conn_str = f"DRIVER={driver};SERVER={server};DATABASE=master;Trusted_Connection=yes;TrustServerCertificate=yes;"
        else:
            conn_str = f"DRIVER={driver};SERVER={server};DATABASE=master;UID={user};PWD={pwd};TrustServerCertificate=yes;"

        self.show_progress(f"Connecting to {server}...")

        def _connect_task():
            # Try connecting to validate string
            sm = SqlManager(conn_str)
            try:
                sm._get_connection().close()
            except Exception:
                # Fallback to standard driver if ODBC 17 is missing
                if win_auth:
                    conn_str_fallback = f"DRIVER={{SQL Server}};SERVER={server};DATABASE=master;Trusted_Connection=yes;"
                else:
                    conn_str_fallback = f"DRIVER={{SQL Server}};SERVER={server};DATABASE=master;UID={user};PWD={pwd};"
                sm = SqlManager(conn_str_fallback)
                sm._get_connection().close() # validate again

            version, mem, cpus = sm.get_server_system_info()
            short_v = sm.get_sql_short_version()
            return sm, version, mem, cpus, short_v

        self.conn_worker = Worker(_connect_task)
        self.conn_worker.finished.connect(self._on_connected)
        self.conn_worker.error.connect(self._on_connect_error)
        self.conn_worker.start()

    def _on_connected(self, result):
        sm, version, mem, cpus, short_v = result
        self.sql_manager = sm
        self._sql_short_version = short_v

        single_line_v = " ".join(version.split())
        self.lbl_sql_version.setText(f"SQL Version: {single_line_v}")
        self.lbl_sql_version.setToolTip(version)

        server_info = f"Server Hardware: {cpus} CPUs | {mem:,} MB RAM"

        client_info = getattr(self, 'client_info_cache', 'Client Hardware: N/A')
        self.txt_system_details.setText(f"{server_info}\n---\n{client_info}")

        self.log("Connection successful.")
        self.set_connected_state()
        self.refresh_database_list()

    def _on_connect_error(self, err_msg):
        self.log(f"ERROR: {err_msg}")
        QMessageBox.critical(self, "Connection Failed", str(err_msg))
        self.set_disconnected_state()

    def refresh_database_list(self):
        if not self.sql_manager: return
        self.show_progress("Refreshing database list...")

        def _refresh_task():
            dbs = self.sql_manager.get_databases_and_files()
            for db in dbs:
                db.mdf_drive_model = SystemInfoHelper.get_drive_model(db.mdf_drive)
                db.ldf_drive_model = SystemInfoHelper.get_drive_model(db.ldf_drive)
            return dbs

        self.refresh_worker = Worker(_refresh_task)
        self.refresh_worker.finished.connect(self._on_db_list_refreshed)
        self.refresh_worker.error.connect(lambda e: (self.log(f"Error refreshing DBs: {e}"), self.hide_progress()))
        self.refresh_worker.start()

    def _on_db_list_refreshed(self, dbs: list[DatabaseInfo]):
        self._databases = dbs
        self.table_databases.setRowCount(0)
        self.cmb_dbs_query.clear()

        for row_index, db in enumerate(dbs):
            self.table_databases.insertRow(row_index)
            self.table_databases.setItem(row_index, 0, QTableWidgetItem(db.name))
            self.cmb_dbs_query.addItem(db.name)

        self.log(f"Found {len(dbs)} databases.")
        self.hide_progress()

    def on_db_selected(self):
        selected_rows = self.table_databases.selectionModel().selectedRows()
        if not selected_rows:
            return
        db_name_item = self.table_databases.item(selected_rows[0].row(), 0)
        if not db_name_item:
            return
        db_name = db_name_item.text()
        db_info = next((db for db in self._databases if db.name == db_name), None)
        if not db_info: return

        self.txt_mdf_path.setText(db_info.mdf_path)
        self.txt_ldf_path.setText(db_info.ldf_path)
        self.lbl_mdf_drive.setText(f"[{db_info.mdf_drive_model}]")
        self.lbl_ldf_drive.setText(f"[{db_info.ldf_drive_model}]")
        self.lbl_db_size.setText(f"Size: {db_info.size_mb:,.2f} MB ({db_info.size_mb/1024.0:,.2f} GB)")
        self.lbl_customer_code.setText(
            f"Customer Code: {db_info.customer_code or 'N/A'}"
        )
        self.lbl_special_category.setText(
            f"Special Category: {db_info.special_category or 'N/A'}"
        )

        # Reset Details UI
        self.lbl_db_version.setText("Data Version: Loading...")
        self.list_fiscal.clear()

        # Fetch details async
        self.detail_worker = Worker(self.sql_manager.get_database_details, db_name)
        self.detail_worker.finished.connect(self._on_details_loaded)
        self.detail_worker.start()

    def _on_details_loaded(self, details: DbDetails):
        self.lbl_db_version.setText(f"Data Version: {details.data_version}")
        self.lbl_company.setText(f"Company: {details.company_name}")
        self.lbl_db_type.setText(f"Type: {details.db_type.value}")
        self.lbl_activation.setText(f"Activation Code: {details.activation_code}")
        self.lbl_access_mode.setText(f"Access Mode: {details.user_access_mode}")

        self.lbl_server_name.setText(f"Server Name: {details.server_name}")
        self.lbl_service_name.setText(f"Service Name: {details.service_name}")
        self.lbl_connections.setText(f"Connections: {details.connections}")
        self.lbl_tran_count.setText(f"Tran Count: {details.tran_count}")
        self.lbl_language.setText(f"Language: {details.language}")
        self.lbl_collation.setText(f"Collation: {details.collation}")

        # Coloring access mode
        mode = details.user_access_mode.upper()
        if mode == "MULTI_USER": self.lbl_access_mode.setStyleSheet("color: darkgreen; font-weight: bold;")
        elif mode == "SINGLE_USER": self.lbl_access_mode.setStyleSheet("color: orangered; font-weight: bold;")
        elif mode == "RESTRICTED_USER": self.lbl_access_mode.setStyleSheet("color: darkorange; font-weight: bold;")
        else: self.lbl_access_mode.setStyleSheet("color: black; font-weight: bold;")

        self.list_fiscal.clear()
        self.list_fiscal.addItems(details.fiscal_years)

    def _get_selected_db_name(self):
        selected_rows = self.table_databases.selectionModel().selectedRows()
        if not selected_rows:
            QMessageBox.warning(self, "Warning", "Please select a database first.")
            return None
        db_name_item = self.table_databases.item(selected_rows[0].row(), 0)
        return db_name_item.text() if db_name_item else None

    def perform_backup(self, verify: bool):
        db_name = self._get_selected_db_name()
        if not db_name: return

        default_name = f"{db_name}_{datetime.now().strftime('%Y%m%d_%H%M%S')}_{self._sql_short_version}.bak"
        file_path, _ = QFileDialog.getSaveFileName(self, "Save Backup As", default_name, "Backup File (*.bak)")
        if not file_path: return

        action = "Backing up and verifying" if verify else "Backing up"
        self.show_progress(f"{action} {db_name} to {file_path}...")

        self.op_worker = Worker(self.sql_manager.backup_database, db_name, file_path, verify)
        self.op_worker.progress.connect(self.progress_bar.setValue)

        def _done(res):
            self.hide_progress()
            msg = "Backup and verification completed successfully!" if verify else "Backup completed successfully!"
            self.log(msg)
            QMessageBox.information(self, "Success", msg)

        self.op_worker.finished.connect(_done)
        self.op_worker.error.connect(lambda e: (self.hide_progress(), self.log(f"ERROR: {e}"), QMessageBox.critical(self, "Error", e)))
        self.op_worker.start()

    def perform_delete(self):
        db_name = self._get_selected_db_name()
        if not db_name: return

        text, ok = QInputDialog.getText(self, "Confirm Delete",
            f"This will permanently delete '{db_name}'.\nType 'Delete' to confirm:")

        if ok and text.strip().lower() == 'delete':
            self.show_progress(f"Deleting database {db_name}...")
            self.op_worker = Worker(self.sql_manager.delete_database, db_name)
            self.op_worker.finished.connect(lambda res: (self.hide_progress(), self.log("Deleted successfully."), self.refresh_database_list()))
            self.op_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", e)))
            self.op_worker.start()
        elif ok:
            self.log(f"Incorrect confirmation text entered. '{db_name}' was not deleted.")

    def perform_detach(self):
        db_name = self._get_selected_db_name()
        if not db_name: return

        text, ok = QInputDialog.getText(self, "Confirm Detach",
            f"This will detach '{db_name}'. Files will not be deleted.\nType 'Detach' to confirm:")

        if ok and text.strip().lower() == 'detach':
            self.show_progress(f"Detaching database {db_name}...")
            self.op_worker = Worker(self.sql_manager.detach_database, db_name)
            self.op_worker.finished.connect(lambda res: (self.hide_progress(), self.log("Detached successfully."), self.refresh_database_list()))
            self.op_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", e)))
            self.op_worker.start()
        elif ok:
            self.log(f"Incorrect confirmation text. '{db_name}' was not detached.")

    def perform_restore(self):
        file_path, _ = QFileDialog.getOpenFileName(self, "Select Backup to Restore", "", "Backup File (*.bak)")
        if not file_path: return

        self.show_progress(f"Analyzing backup file: {file_path}")

        def _analyze_cb(original_name: str):
            self.hide_progress()
            if not original_name:
                QMessageBox.critical(self, "Error", "Could not determine the database name from the backup file.")
                return

            new_db_name, ok = QInputDialog.getText(self, "Restore Database As", "Enter new name:", text=original_name)
            if not ok or not new_db_name.strip(): return

            dest_folder = QFileDialog.getExistingDirectory(self, "Select a folder to save new MDF and LDF files.")
            if not dest_folder: return

            # Normalize the path to convert forward slashes to native Windows backslashes
            dest_folder = os.path.normpath(dest_folder)

            new_mdf = os.path.join(dest_folder, f"{new_db_name.strip()}.mdf")
            new_ldf = os.path.join(dest_folder, f"{new_db_name.strip()}_log.ldf")

            self.show_progress(f"Restoring {new_db_name}...")
            self.log(f"New MDF: {new_mdf}\nNew LDF: {new_ldf}")

            self.restore_worker = Worker(self.sql_manager.restore_database, new_db_name.strip(), file_path, new_mdf, new_ldf)
            self.restore_worker.progress.connect(self.progress_bar.setValue)
            self.restore_worker.finished.connect(lambda res: (self.hide_progress(), QMessageBox.information(self, "Success", "Database restored successfully!"), self.refresh_database_list()))
            self.restore_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", str(e))))
            self.restore_worker.start()

        self.analyze_worker = Worker(self.sql_manager.get_original_database_name, file_path)
        self.analyze_worker.finished.connect(_analyze_cb)
        self.analyze_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", str(e))))
        self.analyze_worker.start()

    def perform_health_check(self):
        db_name = self._get_selected_db_name()
        if not db_name: return
        self.show_progress(f"Running DBCC CHECKDB for {db_name}...")

        def _cb(res):
            output, has_err = res
            self.hide_progress()
            self.log(f"--- DBCC Results for {db_name} ---\n{output}\n--- End of Results ---")
            if has_err: QMessageBox.critical(self, "Health Check: FAILED", f"DBCC CHECKDB found errors in '{db_name}'.")
            else: QMessageBox.information(self, "Health Check: Passed", f"DBCC CHECKDB found no errors in '{db_name}'.")

        self.health_worker = Worker(self.sql_manager.check_database, db_name)
        self.health_worker.finished.connect(_cb)
        self.health_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", str(e))))
        self.health_worker.start()

    def perform_schema_check(self):
        db_name = self._get_selected_db_name()
        if not db_name: return
        self.show_progress(f"Checking table schema for {db_name}...")

        def _cb(res):
            is_match, msg = res
            self.hide_progress()
            self.log(msg.replace("\n", " "))
            if is_match: QMessageBox.information(self, "Schema Check: Passed", msg)
            else: QMessageBox.warning(self, "Schema Check: Mismatch Found", msg)

        self.schema_worker = Worker(self.sql_manager.check_database_schema, db_name)
        self.schema_worker.finished.connect(_cb)
        self.schema_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", str(e))))
        self.schema_worker.start()

    def perform_trigger_check(self):
        db_name = self._get_selected_db_name()
        if not db_name: return
        self.show_progress(f"Checking triggers for {db_name}...")

        def _cb(res):
            is_ok, msg = res
            self.hide_progress()
            self.log(msg.replace("\n", " "))
            if is_ok: QMessageBox.information(self, "Trigger Check: Passed", msg)
            else: QMessageBox.warning(self, "Trigger Check: Issues Found", msg)

        self.trigger_worker = Worker(self.sql_manager.check_database_triggers, db_name)
        self.trigger_worker.finished.connect(_cb)
        self.trigger_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", str(e))))
        self.trigger_worker.start()

    def open_mdf_directory(self):
        path = self.txt_mdf_path.text()
        if path and os.path.exists(os.path.dirname(path)):
            os.startfile(os.path.dirname(path))

    def browse_file(self, line_edit: QLineEdit, filter_str: str):
        path, _ = QFileDialog.getOpenFileName(self, "Select File", "", filter_str)
        if path:
            # Fix: Normalize forward slashes to backslashes for the UI
            path = os.path.normpath(path)
            line_edit.setText(path)
            # Auto fill LDF if MDF is selected
            if "MDF" in filter_str.upper():
                ldf_guess = path.replace(".mdf", "_log.ldf")
                if self.txt_attach_ldf.text() == "":
                    self.txt_attach_ldf.setText(ldf_guess)

    def perform_attach(self):
        mdf = self.txt_attach_mdf.text().strip()
        ldf = self.txt_attach_ldf.text().strip()

        # Fix: Normalize the final strings before passing to SQL Server
        if mdf: mdf = os.path.normpath(mdf)
        if ldf: ldf = os.path.normpath(ldf)

        if not mdf:
            QMessageBox.warning(self, "Warning", "Please provide MDF path.")
            return

        db_name = os.path.splitext(os.path.basename(mdf))[0]
        self.show_progress(f"Attaching {db_name}...")

        self.attach_worker = Worker(self.sql_manager.attach_database, db_name, mdf, ldf)
        self.attach_worker.finished.connect(lambda res: (
            self.hide_progress(),
            QMessageBox.information(self, "Success", "Database attached successfully!"),
            self.txt_attach_mdf.clear(),
            self.txt_attach_ldf.clear(),
            self.refresh_database_list()
        ))
        self.attach_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Error", str(e))))
        self.attach_worker.start()

    def browse_servers(self):
        """Uses local WMI to find SQL Server services as network discovery requires UDP broadcasts."""
        self.show_progress("Scanning local services for SQL Server instances...")
        def _scan():
            pythoncom.CoInitialize()
            try:
                c = wmi.WMI()
                instances = []
                for service in c.Win32_Service(Name="MSSQLSERVER"): instances.append(".\\MSSQLSERVER")
                for service in c.Win32_Service():
                    if service.Name.startswith("MSSQL$"):
                        instances.append(f".\\{service.Name.split('$')[1]}")
                return instances if instances else [".\\SQLEXPRESS"]
            finally:
                pythoncom.CoUninitialize()

        self.scan_worker = Worker(_scan)
        self.scan_worker.finished.connect(lambda res: (self.hide_progress(), self.show_server_selector(res)))
        self.scan_worker.error.connect(lambda e: self.hide_progress())
        self.scan_worker.start()

    def show_server_selector(self, servers):
        if not servers:
            QMessageBox.information(self, "Not Found", "No local instances discovered.")
            return
        item, ok = QInputDialog.getItem(self, "Select SQL Server", "Local Instances:", servers, 0, False)
        if ok and item: self.txt_server.setText(item)

    def open_query(self):
        path, _ = QFileDialog.getOpenFileName(self, "Open Query", "", "SQL File (*.sql);;All Files (*.*)")
        if path:
            try:
                with open(path, 'r', encoding='utf-8') as f:
                    self.txt_query.setPlainText(f.read())
                self.log(f"Query loaded from {path}")
            except Exception as e:
                QMessageBox.critical(self, "Error", f"Could not load query: {e}")

    def save_query(self):
        text = self.txt_query.toPlainText()
        if not text.strip(): return
        path, _ = QFileDialog.getSaveFileName(self, "Save Query", "", "SQL File (*.sql);;All Files (*.*)")
        if path:
            try:
                with open(path, 'w', encoding='utf-8') as f:
                    f.write(text)
                self.log(f"Query saved to {path}")
            except Exception as e:
                QMessageBox.critical(self, "Error", f"Could not save query: {e}")

    def execute_query(self):
        if not self.sql_manager:
            QMessageBox.warning(self, "Warning", "Please connect to a server first.")
            return

        db_name = self.cmb_dbs_query.currentText()
        if not db_name:
            QMessageBox.warning(self, "Warning", "Please select a database.")
            return

        # Get selected text if exists, else all text
        cursor = self.txt_query.textCursor()
        query = cursor.selectedText() if cursor.hasSelection() else self.txt_query.toPlainText()

        if not query.strip(): return

        self.show_progress("Executing query...")
        self.dgv_results.clear()
        self.dgv_results.setRowCount(0)
        self.dgv_results.setColumnCount(0)

        def _cb(res):
            columns, rows = res
            self.hide_progress()
            self.log(f"Query executed successfully. {len(rows)} rows returned.")

            self.dgv_results.setColumnCount(len(columns))
            self.dgv_results.setHorizontalHeaderLabels(columns)
            self.dgv_results.setRowCount(len(rows))

            for row_idx, row_data in enumerate(rows):
                for col_idx, value in enumerate(row_data):
                    item = QTableWidgetItem(str(value) if value is not None else "NULL")
                    self.dgv_results.setItem(row_idx, col_idx, item)

            self.dgv_results.horizontalHeader().setSectionResizeMode(QHeaderView.ResizeMode.ResizeToContents)

        self.exec_worker = Worker(self.sql_manager.execute_query, query, db_name)
        self.exec_worker.finished.connect(_cb)
        self.exec_worker.error.connect(lambda e: (self.hide_progress(), QMessageBox.critical(self, "Query Error", str(e)), self.log(f"Error: {e}")))
        self.exec_worker.start()

    def keyPressEvent(self, event):
        if event.key() == Qt.Key.Key_F5 and self.tab_widget.currentIndex() == 1:
            self.execute_query()
            event.accept()
        else:
            super().keyPressEvent(event)

def main():
    app = QApplication(sys.argv)
    app.setStyle("Fusion")

    # Load Application Icons (Favicon, Taskbar, Window Header)
    app_icon = QIcon()

    # PyInstaller creates a temp folder and stores path in _MEIPASS
    if getattr(sys, 'frozen', False):
        base_dir = sys._MEIPASS
    else:
        base_dir = os.path.dirname(os.path.abspath(__file__))

    icon_dir = os.path.join(base_dir, "icons")

    if os.path.exists(icon_dir):
        # Load all resolution variants from the icons folder
        for file in os.listdir(icon_dir):
            if file.lower().endswith(('.png', '.ico')):
                app_icon.addFile(os.path.join(icon_dir, file))
        app.setWindowIcon(app_icon)

    # Modern styling defaults
    palette = QPalette()
    palette.setColor(QPalette.ColorRole.Window, QColor(240, 240, 240))
    palette.setColor(QPalette.ColorRole.WindowText, Qt.GlobalColor.black)
    app.setPalette(palette)

    if REQUIRE_ACCESS_PASSWORD and not request_time_password():
        return

    window = MainWindow()
    window.setWindowIcon(app_icon) # Apply explicitly to main window
    window.show()
    sys.exit(app.exec())

if __name__ == "__main__":
    main()
