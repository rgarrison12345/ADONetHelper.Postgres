#region Licenses
/*MIT License
Copyright(c) 2020
Robert Garrison

Permission Is hereby granted, free Of charge, To any person obtaining a copy
of this software And associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
copies Of the Software, And To permit persons To whom the Software Is
furnished To Do so, subject To the following conditions:

The above copyright notice And this permission notice shall be included In all
copies Or substantial portions Of the Software.

THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
FITNESS For A PARTICULAR PURPOSE And NONINFRINGEMENT. In NO Event SHALL THE
AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
OUT Of Or In CONNECTION With THE SOFTWARE Or THE USE Or OTHER DEALINGS In THE
SOFTWARE*/
#endregion
#region Using Statements
using ADONetHelper.Core;
using Npgsql;
using Npgsql.TypeMapping;
using System;
using System.Data;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace ADONetHelper.Postgres
{
    /// <summary>
    /// A specialized instance of <see cref="DbClient"/> that is used to query a Postgres database system
    /// </summary>
    /// <seealso cref="DbClient"/>
    public class PostgresClient : DbClient
    {
        #region Events
        /// <summary>
        /// Occurs on NoticeResponses from the PostgreSQL backend.
        /// </summary>
        /// <value>
        /// Represents the method that handles the <see cref="Notification"/> events.
        /// </value>
        public event NoticeEventHandler Notice
        {
            add
            {
                //Get an exclusive lock
                lock (Connection)
                {
                    Connection.Notice += value;
                }
            }
            remove
            {
                //Get an exclusive lock
                lock (Connection)
                {
                    Connection.Notice -= value;
                }
            }
        }
        /// <summary>
        /// Occurs on NotificationResponses from the PostgreSQL backend.
        /// </summary>
        /// <value>
        /// Represents the method that handles the <see cref="Notification"/> events.
        /// </value>
        public event NotificationEventHandler Notification
        {
            add
            {
                //Get an exclusive lock
                lock (Connection)
                {
                    Connection.Notification += value;
                }
            }
            remove
            {
                //Get an exclusive lock
                lock (Connection)
                {
                    Connection.Notification -= value;
                }
            }
        }
        #endregion
        #region Fields/Properties        
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        protected NpgsqlConnection Connection
        {
            get
            {
                //Return this back to the caller
                return (NpgsqlConnection)ExecuteSQL.Connection;
            }
        }
        /// <summary>
        /// The connection specific type mapper, all modifications affect the current connection only and all changes are lost when the connection closes
        /// </summary>
        public INpgsqlTypeMapper TypeMapper
        {
            get
            {
                return Connection.TypeMapper;
            }
        }
        /// <summary>
        /// Gets or sets the delegate used to generate a password for new database connections
        /// </summary>
        public ProvidePasswordCallback ProvidePasswordCallback
        {
            get
            {
                return Connection.ProvidePasswordCallback;
            }
            set
            {
                Connection.ProvidePasswordCallback = value;
            }
        }
        /// <summary>
        /// Selects the local Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <value>
        /// An instance of <see cref="ProvideClientCertificatesCallback"/>
        /// </value>
        /// <seealso cref="Npgsql.ProvideClientCertificatesCallback"/>
        public ProvideClientCertificatesCallback ProvideClientCertificatesCallback
        {
            get
            {
                //Return this back to the caller
                return Connection.ProvideClientCertificatesCallback;
            }
            set
            {
                //Set the value
                Connection.ProvideClientCertificatesCallback = value;
            }
        }
        /// <summary>
        /// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication. 
        /// Ignored if TrustServerCertificate is set.
        /// </summary>
        /// <value>
        /// An instance of <see cref="RemoteCertificateValidationCallback"/>
        /// </value>
        public RemoteCertificateValidationCallback UserCertificateValidationCallback
        {
            get
            {
                //Return this back to the caller
                return Connection.UserCertificateValidationCallback;
            }
            set
            {
                Connection.UserCertificateValidationCallback = value;
            }
        }
        /// <summary>
        /// Returns The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
        /// </summary>
        /// <value>
        /// The connection's timezone as reported by PostgreSQL, in the IANA/Olson database format.
        /// </value>
        public string Timezone
        {
            get
            {
                //Return this back to the caller
                return Connection.Timezone;
            }
        }
        /// <summary>
        /// Reports whether the backend uses the newer integer timestamp representation. 
        /// Note that the old floating point representation is not supported. Meant for use by type plugins (e.g. Nodatime)
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has integer date times; otherwise, <c>false</c>.
        /// </value>
        public bool HasIntegerDateTimes
        {
            get
            {
                //Return this back to the caller
                return Connection.HasIntegerDateTimes;
            }
        }
        /// <summary>
        /// The port number of the backend server
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port
        {
            get
            {
                //Return this back to the caller
                return Connection.Port;
            }
        }
        /// <summary>
        /// Gets the postgres version.
        /// </summary>
        /// <value>
        /// The postgres version.
        /// </value>
        public Version PostgresVersion
        {
            get
            {
                //Return this back to the caller
                return Connection.PostgreSqlVersion;
            }
        }
        /// <summary>
        /// Gets the name of the backend host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName
        {
            get
            {
                //Return this back to the caller
                return Connection.Host;
            }
        }
        /// <summary>
        /// Gets the process identifier.
        /// </summary>
        /// <value>
        /// The process identifier.
        /// </value>
        public int ProcessID
        {
            get
            {
                //Return this back to the caller
                return Connection.ProcessID;
            }
        }
        /// <summary>
        /// Gets a value indicating whether windows authentication is required to log in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required to log in; otherwise, <c>false</c>.
        /// </value>
        public bool IntegratedSecurity
        {
            get
            {
                //Return this back to the caller
                return Connection.IntegratedSecurity;
            }
        }
        /// <summary>
        /// The name of the user from the connection string
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get
            {
                //Return this back to the caller
                return Connection.UserName;
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// The overloaded constuctor that will initialize the <paramref name="connectionString"/>, And <paramref name="queryCommandType"/>
        /// </summary>
        /// <param name="connectionString">The connection string used to query a data store</param>
        /// <param name="queryCommandType">Represents how a command should be interpreted by the data provider</param>
        public PostgresClient(string connectionString, CommandType queryCommandType) : base(connectionString, queryCommandType, NpgsqlFactory.Instance)
        {

        }
        /// <summary>
        /// The overloaded constuctor that will initialize the <paramref name="connectionString"/>
        /// </summary>
        /// <param name="connectionString">The connection string used to query a data store</param>
        public PostgresClient(string connectionString) : base(connectionString, NpgsqlFactory.Instance)
        {
        }
        /// <summary>
        /// Intializes the <see cref="PostgresClient"/> with a <see cref="ISqlExecutor"/>
        /// </summary>
        /// <param name="executor">An instance of <see cref="ISqlExecutor"/></param>
        public PostgresClient(ISqlExecutor executor) : base(executor)
        {
        }
        /// <summary>
        /// Insantiates a new instance of <see cref="PostgresClient"/> using the passed in <paramref name="connectionString"/> and <paramref name="factory"/>
        /// </summary>
        /// <param name="connectionString">Connection string to use to query a database</param>
        /// <param name="factory">An instance of <see cref="IDbObjectFactory"/></param>
        public PostgresClient(string connectionString, IDbObjectFactory factory) : base(connectionString, factory)
        {
        }
        /// <summary>
        /// Constructor to query a database using an existing <see cref="NpgsqlConnection"/> to initialize the <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">An instance of <see cref="NpgsqlConnection"/> to use to query a database </param>
        public PostgresClient(NpgsqlConnection connection) : base(connection)
        {
        }
        /// <summary>
        /// Constructor to query a database using an existing <see cref="NpgsqlConnection"/> to initialize the <paramref name="connection"/> and <paramref name="commandType"/>
        /// </summary>
        /// <param name="commandType">Represents how a command should be interpreted by the data provider</param>
        /// <param name="connection">An instance of <see cref="NpgsqlConnection"/> to use to query a database </param>
        public PostgresClient(NpgsqlConnection connection, CommandType commandType) : base(connection, commandType)
        {
        }
        #endregion
        #region Utility Methods
#if !NET461 && !NETSTANDARD2_0
        /// <summary>
        /// Begins the transaction asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async ValueTask<NpgsqlTransaction> BeginTransactionAsync(CancellationToken token = default)
        {
            //Return this back to the caller
            return await Connection.BeginTransactionAsync(token).ConfigureAwait(false);
        }
        /// <summary>
        /// Begins the transaction asynchronous.
        /// </summary>
        /// <param name="levl">The levl.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async ValueTask<NpgsqlTransaction> BeginTransactionAsync(IsolationLevel levl, CancellationToken token = default)
        {
            //Return this back to the caller
            return await Connection.BeginTransactionAsync(levl, token).ConfigureAwait(false);
        }
#endif
        /// <summary>
        /// Closes the asynchronous.
        /// </summary>
        public async Task CloseAsync()
        {
            await Connection.CloseAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Unprepares all statements on the current <see cref="NpgsqlConnection"/>
        /// </summary>
        public void UnPrepareAllStatements()
        {
            Connection.UnprepareAll();
        }
        /// <summary>
        /// Waits this instance.
        /// </summary>
        public void Wait()
        {
            //Go ahead and wait
            Connection.Wait();
        }
        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void Wait(int timeout)
        {
            //Go ahead and wait for an event
            Connection.Wait(timeout);
        }
        /// <summary>
        /// Waits the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void Wait(TimeSpan timeout)
        {
            Connection.Wait(timeout);
        }
        /// <summary>
        /// Waits the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public async Task WaitAsync(CancellationToken token = default)
        {
            //Go ahead and wait for an event
            await Connection.WaitAsync(token).ConfigureAwait(false);
        }
        /// <summary>
        /// Flushes the type cache for this <see cref="NpgsqlConnection"/> <see cref="NpgsqlConnection.ConnectionString"/> and reloads types for this connection only
        /// </summary>
        public void ReloadTypes()
        {
            //Go and reload the types for this connection
            Connection.ReloadTypes();
        }
        /// <summary>
        /// Gets the raw copy stream.
        /// </summary>
        /// <param name="copyCommand">The copy command.</param>
        /// <returns></returns>
        public NpgsqlRawCopyStream GetRawCopyStream(string copyCommand)
        {
            //Return this back to the caller
            return Connection.BeginRawBinaryCopy(copyCommand);
        }
        /// <summary>
        /// Gets the text exporter.
        /// </summary>
        /// <param name="copyToCommand">The copy to command.</param>
        /// <returns></returns>
        public NpgsqlCopyTextReader GetTextExporter(string copyToCommand)
        {
            //Return this back to the caller
            return (NpgsqlCopyTextReader)Connection.BeginTextExport(copyToCommand);
        }
        /// <summary>
        /// Gets the text importer.
        /// </summary>
        /// <param name="copyFromCommand">The copy from command.</param>
        /// <returns></returns>
        public NpgsqlCopyTextWriter GetTextImporter(string copyFromCommand)
        {
            //Return this back to the caller
            return (NpgsqlCopyTextWriter)Connection.BeginTextImport(copyFromCommand);
        }
        /// <summary>
        /// Gets the binary exporter.
        /// </summary>
        /// <param name="copyToCommand">The copy to command.</param>
        /// <returns></returns>
        public NpgsqlBinaryExporter GetBinaryExporter(string copyToCommand)
        {
            //Return this back to the caller
            return Connection.BeginBinaryExport(copyToCommand);
        }
        /// <summary>
        /// Gets the binary importer.
        /// </summary>
        /// <param name="copyFromCommand">The copy from command.</param>
        /// <returns></returns>
        public NpgsqlBinaryImporter GetBinaryImporter(string copyFromCommand)
        {
            //Return this back to the caller
            return Connection.BeginBinaryImport(copyFromCommand);
        }
        #endregion
    }
}