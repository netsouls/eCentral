using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using eCentral.Data.Initializers;

namespace eCentral.Data
{
    public class SqlServerDataProvider : BaseEfDataProvider
    {
        /// <summary>
        /// Get connection factory
        /// </summary>
        /// <returns>Connection factory</returns>
        public override IDbConnectionFactory GetConnectionFactory()
        {
            return new SqlConnectionFactory();
        }

        /// <summary>
        /// Set database initializer
        /// </summary>
        public override void SetDatabaseInitializer()
        {
            //pass some table names to ensure that we have the system installed
            var tablesToValidate = new[] { "Port" };

            //custom commands (stored proedures, indexes)
            var customCommands = new List<string>();

            /* 
             * Commenting the below installation process as this is not required
             * 
             * customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.Functions.sql"), false));
             * customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.Indexes.sql"), false));
             * customCommands.AddRange(ParseCommands(HostingEnvironment.MapPath("~/App_Data/SqlServer.StoredProcedures.sql"), false));
            */

            var initializer = new CreateTablesIfNotExist<SiteObjectContext>(tablesToValidate, customCommands.ToArray());
            Database.SetInitializer(initializer);
        }

        protected virtual string[] ParseCommands(string filePath, bool throwExceptionIfNonExists)
        {
            if (!File.Exists(filePath))
            {
                if (throwExceptionIfNonExists)
                    throw new ArgumentException(string.Format("Specified file doesn't exist - {0}", filePath));
                else
                    return new string[0];
            }


            var statements = new List<string>();
            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                var statement = "";
                while ((statement = readNextStatementFromStream(reader)) != null)
                {
                    statements.Add(statement);
                }
            }

            return statements.ToArray();
        }

        protected virtual string readNextStatementFromStream(StreamReader reader)
        {
            var sb = new StringBuilder();

            string lineOfText;

            while (true)
            {
                lineOfText = reader.ReadLine();
                if (lineOfText == null)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    else
                        return null;
                }

                if (lineOfText.TrimEnd().ToUpper() == "GO")
                    break;

                sb.Append(lineOfText + Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// A value indicating whether this data provider supports stored procedures
        /// </summary>
        public override bool StoredProceduredSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public override DbParameter GetParameter()
        {
            return new SqlParameter();
        }
    }
}
