using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Collections;

namespace EasyWay.CoreDataLayer
{
    
   public class CoreDataLayer 
    {

        protected DbConnection database_ = null;
        protected SqlConnection connectionTrans_ = null;
        protected SqlTransaction transaction_ = null;

        protected bool isInTransaction_ = false;

        public bool IsInTransaction
		{
			get { return isInTransaction_; }
		}

        public CoreDataLayer()
		{
				database_ = new DbConnection();
			
		}
		
		/// <summary>
		/// Releases all file handles and all unmanaged memory resource 
		/// </summary>
		public void Dispose()
		{
			if(this.connectionTrans_!=null)
				this.connectionTrans_.Dispose();
			
		}

        public void BeginTransaction()
        {
            try
            {
                if (isInTransaction_ != true)
                {

                    connectionTrans_ = database_.GetConnection();
                    connectionTrans_.Open();

                    transaction_ = connectionTrans_.BeginTransaction();
                    isInTransaction_ = true;

                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

            finally
            {

            }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (connectionTrans_ != null && transaction_ != null)
                {
                    transaction_.Rollback();
                    isInTransaction_ = false;
                    connectionTrans_.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                if (transaction_ != null)
                    transaction_ = null;
                if (connectionTrans_ != null)
                    connectionTrans_.Dispose();
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (connectionTrans_ != null && transaction_ != null)
                {
                    transaction_.Commit();
                    isInTransaction_ = false;
                    connectionTrans_.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;


            }
            finally
            {
                if (transaction_ != null)
                    transaction_ = null;
                if (connectionTrans_ != null)
                    connectionTrans_.Dispose();
            }
        }

        /// <summary>
        ///  Executes the stored procedure and return resultset as dataset
        /// </summary>
        /// <param name="StoredProcedureName"></param>
        /// <param name="ParameterValues"></param>
        /// <param name="arrParamNames"></param>
        /// <param name="arrParamTypes"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string StoredProcedureName, object[] ParameterValues, object[] arrParamNames, object[] arrParamTypes)
        {
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter();
                if (StoredProcedureName.IndexOf("dbo.") < 0)
                    StoredProcedureName = "dbo." + StoredProcedureName;
                try
                {

                    if (!isInTransaction_)
                    {
                        DbConnection oDbConnection = new DbConnection();
                        using (SqlConnection conn = oDbConnection.GetConnection())
                        {
                            conn.Open();
                            using (SqlCommand objCommand = new SqlCommand())
                            {
                                objCommand.Connection = conn;
                                objCommand.CommandText = StoredProcedureName;
                                objCommand.CommandType = CommandType.StoredProcedure;
                                objCommand.CommandTimeout = 240;


                                for (int i = 0; i < arrParamNames.Length; i++)
                                {
                                    SqlParameter objParam = new SqlParameter(arrParamNames[i].ToString(), arrParamTypes[i].ToString());
                                    objParam.Value = ParameterValues[i].ToString();
                                    objCommand.Parameters.Add(objParam);
                                }

                                SqlDataAdapter objDataAdapter = new SqlDataAdapter();

                                objDataAdapter.SelectCommand = objCommand;
                                //retVal = Convert.ToInt64(objCommand.ExecuteNonQuery());
                                objDataAdapter.Fill(ds, "Results");
                            }

                        }

                        return ds;
                    }
                    else
                    {
                        using (SqlCommand objCommand = new SqlCommand())
                        {
                            objCommand.Connection = connectionTrans_;
                            objCommand.CommandText = StoredProcedureName;
                            objCommand.Transaction = transaction_;
                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.CommandTimeout = 240;

                            for (int i = 0; i < arrParamNames.Length; i++)
                            {
                                SqlParameter objParam = new SqlParameter(arrParamNames[i].ToString(), arrParamTypes[i].ToString());
                                objParam.Value = ParameterValues[i].ToString();
                                objCommand.Parameters.Add(objParam);
                            }

                            SqlDataAdapter objDataAdapter = new SqlDataAdapter();

                            objDataAdapter.SelectCommand = objCommand;
                            //retVal = Convert.ToInt64(objCommand.ExecuteNonQuery());
                            objDataAdapter.Fill(ds, "Results");
                        }


                        return ds;
                    }
                    

                }
                catch (Exception ex)
                {
                    throw ex;
                    
                }

        }

       /// <summary>
       /// Executes the stored procedure for DML
       /// </summary>
       /// <param name="StoredProcedureName"></param>
       /// <param name="ParameterValues"></param>
       /// <param name="arrParamNames"></param>
       /// <param name="arrParamTypes"></param>
        public void ExecuteNonQuery(string StoredProcedureName, object[] ParameterValues, object[] arrParamNames, object[] arrParamTypes)
        {
            
                try
                {
                    if (StoredProcedureName.IndexOf("dbo.") < 0)
                        StoredProcedureName = "dbo." + StoredProcedureName;
                    if (!isInTransaction_)
                    {
                        DbConnection oDbConnection = new DbConnection();
                        using (SqlConnection conn = oDbConnection.GetConnection())
                        {
                            conn.Open();
                            using (SqlCommand objCommand = new SqlCommand())
                            {
                                objCommand.Connection = conn;

                                objCommand.CommandText = StoredProcedureName;

                                objCommand.CommandType = CommandType.StoredProcedure;
                                objCommand.CommandTimeout = 240;

                                //SqlCommandBuilder.DeriveParameters(objCommand);

                                for (int i = 0; i < arrParamNames.Length; i++)
                                {
                                    SqlParameter objParam = new SqlParameter(arrParamNames[i].ToString(), arrParamTypes[i].ToString());
                                    objParam.Value = ParameterValues[i].ToString();
                                    objCommand.Parameters.Add(objParam);
                                }

                                objCommand.ExecuteNonQuery();
                            }



                        }
                    }
                    else
                    {
                        using (SqlCommand objCommand = new SqlCommand())
                        {
                            objCommand.Connection = connectionTrans_;

                            objCommand.CommandText = StoredProcedureName;
                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.Transaction = transaction_;
                            objCommand.CommandTimeout = 240;


                            for (int i = 0; i < arrParamNames.Length; i++)
                            {
                                SqlParameter objParam = new SqlParameter(arrParamNames[i].ToString(), arrParamTypes[i].ToString());
                                objParam.Value = ParameterValues[i].ToString();
                                objCommand.Parameters.Add(objParam);
                            }

                            objCommand.ExecuteNonQuery();
                        }


                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return;
        }


        public DataSet ExecuteDataSet(string StoredProcedureName, object[] ParameterValues)
        {

           
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter();
                if (StoredProcedureName.IndexOf("dbo.") < 0)
                    StoredProcedureName = "dbo." + StoredProcedureName;
                try
                {

                    if (!isInTransaction_)
                    {

                        DbConnection oDbConnection = new DbConnection();
                        using (SqlConnection oConn = oDbConnection.GetConnection())
                        {
                            oConn.Open();

                            using (SqlCommand objCommand = new SqlCommand())
                            {
                                objCommand.Connection = oConn;

                                objCommand.CommandText = StoredProcedureName;

                                objCommand.CommandType = CommandType.StoredProcedure;
                                objCommand.CommandTimeout = 240;

                                SqlCommandBuilder.DeriveParameters(objCommand);

                                int i = 0;
                                foreach (SqlParameter sp in objCommand.Parameters)
                                {
                                    if (sp.ParameterName == "@RETURN_VALUE")
                                        continue;

                                    sp.Value = ParameterValues[i];
                                    i++;
                                }

                                SqlDataAdapter objDataAdapter = new SqlDataAdapter();

                                objDataAdapter.SelectCommand = objCommand;
                                objDataAdapter.Fill(ds, "Results");
                            }

                        }

                        return ds;
                    }
                    else
                    {

                        using (SqlCommand objCommand = new SqlCommand())
                        {
                            objCommand.Connection = connectionTrans_;

                            objCommand.CommandText = StoredProcedureName;
                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.Transaction = transaction_;
                            objCommand.CommandTimeout = 240;


                            SqlCommandBuilder.DeriveParameters(objCommand);

                            int i = 0;
                            foreach (SqlParameter sp in objCommand.Parameters)
                            {
                                if (sp.ParameterName == "@RETURN_VALUE")
                                    continue;
                                sp.Value = ParameterValues[i];
                                i++;


                            }

                            SqlDataAdapter objDataAdapter = new SqlDataAdapter();

                            objDataAdapter.SelectCommand = objCommand;

                            objDataAdapter.Fill(ds, "Results");
                        }
                        return ds;
                    }

                   
                }
                catch (Exception ex)
                {
                   
                    throw ex;


                }

            

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StoredProcedureName"></param>
        /// <param name="ParameterValues"></param>
        /// <param name="isNonQuery">bool isNonQuery - True (returns if insert, update or deleted statements success or not)
        /// - False (returns the first row of first column result value)</param>
        /// <returns></returns>
        public string ExecuteScalar(string StoredProcedureName, object[] ParameterValues, bool isNonQuery)
        {
            
                string retValue = string.Empty;

                try
                {

                    if (StoredProcedureName.IndexOf("dbo.") < 0)
                        StoredProcedureName = "dbo." + StoredProcedureName;
                    if (!isInTransaction_)
                    {
                        DbConnection oDbConnection = new DbConnection();
                        using (SqlConnection conn = oDbConnection.GetConnection())
                        {
                            conn.Open();
                            using (SqlCommand objCommand = new SqlCommand())
                            {
                                objCommand.Connection = conn;

                                objCommand.CommandText = StoredProcedureName;

                                objCommand.CommandType = CommandType.StoredProcedure;
                                objCommand.CommandTimeout = 240;

                                SqlCommandBuilder.DeriveParameters(objCommand);

                                int i = 0;
                                foreach (SqlParameter sp in objCommand.Parameters)
                                {
                                    if (sp.ParameterName == "@RETURN_VALUE")
                                    {
                                        sp.Direction = ParameterDirection.ReturnValue;
                                        continue;
                                    }
                                    
                                        sp.Value = ParameterValues[i];
                                    

                                    i++;


                                }

                                if (isNonQuery)
                                {
                                    objCommand.ExecuteNonQuery();
                                    if (objCommand.Parameters["@RETURN_VALUE"] != null && objCommand.Parameters["@RETURN_VALUE"].Value != null)
                                        retValue = objCommand.Parameters["@RETURN_VALUE"].Value.ToString();

                                }
                                else
                                {
                                    retValue = objCommand.ExecuteScalar().ToString();
                                }
                            }


                        }

                    }
                    else
                    {




                        using (SqlCommand objCommand = new SqlCommand())
                        {
                            objCommand.Connection = connectionTrans_;
                            objCommand.CommandText = StoredProcedureName;
                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.Transaction = transaction_;
                            objCommand.CommandTimeout = 240;


                            SqlCommandBuilder.DeriveParameters(objCommand);

                            int i = 0;

                            foreach (SqlParameter sp in objCommand.Parameters)
                            {
                                if (sp.ParameterName == "@RETURN_VALUE")
                                {
                                    sp.Direction = ParameterDirection.ReturnValue;
                                    continue;
                                }
                                
                                 sp.Value = ParameterValues[i];
                                

                                i++;

                            }

                            if (isNonQuery)
                            {
                                objCommand.ExecuteNonQuery();
                                if (objCommand.Parameters["@RETURN_VALUE"] != null && objCommand.Parameters["@RETURN_VALUE"].Value != null)
                                    retValue = objCommand.Parameters["@RETURN_VALUE"].Value.ToString();

                            }
                            else
                            {
                                retValue = objCommand.ExecuteScalar().ToString();
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

             return retValue;
            
        }

       /// <summary>
       /// This will execute nonquery ( insert ,update and delete and returns the result of select statement as scalar first row and column)
       /// </summary>
       /// <param name="StoredProcedureName"></param>
       /// <param name="ParameterValues"></param>
       /// <returns></returns>
        public string ExecuteNonQueryAndScalar(string StoredProcedureName, object[] ParameterValues)
        {

            string retValue = string.Empty;
           
            try
            {

                if (StoredProcedureName.IndexOf("dbo.") < 0)
                    StoredProcedureName = "dbo." + StoredProcedureName;
                if (!isInTransaction_)
                {
                    DbConnection oDbConnection = new DbConnection();
                    using (SqlConnection conn = oDbConnection.GetConnection())
                    {
                        conn.Open();
                        using (SqlCommand objCommand = new SqlCommand())
                        {
                            objCommand.Connection = conn;

                            objCommand.CommandText = StoredProcedureName;

                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.CommandTimeout = 240;

                            SqlCommandBuilder.DeriveParameters(objCommand);

                            int i = 0;
                            foreach (SqlParameter sp in objCommand.Parameters)
                            {
                                if (sp.ParameterName == "@RETURN_VALUE")
                                {
                                    sp.Direction = ParameterDirection.ReturnValue;
                                    continue;
                                }

                                sp.Value = ParameterValues[i];


                                i++;


                            }

                               retValue = objCommand.ExecuteScalar().ToString();
                                
                            
                        }


                    }

                }
                else
                {

                    using (SqlCommand objCommand = new SqlCommand())
                    {
                        objCommand.Connection = connectionTrans_;
                        objCommand.CommandText = StoredProcedureName;
                        objCommand.CommandType = CommandType.StoredProcedure;
                        objCommand.Transaction = transaction_;
                        objCommand.CommandTimeout = 240;


                        SqlCommandBuilder.DeriveParameters(objCommand);

                        int i = 0;

                        foreach (SqlParameter sp in objCommand.Parameters)
                        {
                            if (sp.ParameterName == "@RETURN_VALUE")
                            {
                                sp.Direction = ParameterDirection.ReturnValue;
                                continue;
                            }

                            sp.Value = ParameterValues[i];


                            i++;

                        }

                        objCommand.ExecuteNonQuery();
                        retValue = objCommand.ExecuteScalar().ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retValue;

        }

        /// <summary>
        /// This method is used for inserting and retrieving identity value
        /// </summary>
        /// <param name="StoredProcedureName"></param>
        /// <param name="ParameterValues"></param>
        /// <returns></returns>
        public int ExecuteIdentity(string StoredProcedureName, object[] ParameterValues)
        {
            
            
                int newid = 0;

                try
                {

                    if (StoredProcedureName.IndexOf("dbo.") < 0)
                        StoredProcedureName = "dbo." + StoredProcedureName;
                    if (!isInTransaction_)
                    {
                        DbConnection oDbConnection = new DbConnection();
                        using (SqlConnection conn = oDbConnection.GetConnection())
                        {
                            conn.Open();
                            using (SqlCommand objCommand = new SqlCommand())
                            {
                                objCommand.Connection = conn;

                                objCommand.CommandText = StoredProcedureName;

                                objCommand.CommandType = CommandType.StoredProcedure;
                                objCommand.CommandTimeout = 240;

                                SqlCommandBuilder.DeriveParameters(objCommand);

                                int i = 0;
                                foreach (SqlParameter sp in objCommand.Parameters)
                                {
                                    if (sp.ParameterName == "@RETURN_VALUE")
                                    {
                                        sp.Direction = ParameterDirection.ReturnValue;
                                        continue;
                                    }
                                    
                                        sp.Value = ParameterValues[i];
                                    
                                    i++;


                                }

                                newid = Int32.Parse(objCommand.ExecuteScalar().ToString());
                            }


                        }

                    }
                    else
                    {

                        using (SqlCommand objCommand = new SqlCommand())
                        {
                            objCommand.Connection = connectionTrans_;
                            objCommand.CommandText = StoredProcedureName;
                            objCommand.CommandType = CommandType.StoredProcedure;
                            objCommand.Transaction = transaction_;
                            objCommand.CommandTimeout = 240;


                            SqlCommandBuilder.DeriveParameters(objCommand);

                            int i = 0;

                            foreach (SqlParameter sp in objCommand.Parameters)
                            {
                                if (sp.ParameterName == "@RETURN_VALUE")
                                {
                                    sp.Direction = ParameterDirection.ReturnValue;
                                    continue;
                                }
                                    sp.Value = ParameterValues[i];
  
                                i++;

                            }
                            newid = Int32.Parse(objCommand.ExecuteScalar().ToString());
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                return newid;
            
        }


    }
}
