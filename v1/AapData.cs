using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Collections;
using CDL = EasyWay.CoreDataLayer;
namespace AAPVolunteers
{
    public class AapData
    {

        public DataSet getMedataByType(int typeId,string typeName,int parentId,int isParent)
        {
            DataSet dsMetadata = null;
            CDL.CoreDataLayer objCoreDataLayer = new CDL.CoreDataLayer();
            try
            {
                dsMetadata = new DataSet();
                dsMetadata = objCoreDataLayer.ExecuteDataSet("pr_getmetadatabytypeid", new object[] { typeId, typeName, parentId, isParent });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsMetadata;

        }

        public Boolean verifyVolunteer(string mobileNo,string uCode)
        {
            
            Boolean flag = false;
            CDL.CoreDataLayer objCoreDataLayer = new CDL.CoreDataLayer();
            try
            {
                string strRetValue = string.Empty;

                strRetValue = objCoreDataLayer.ExecuteScalar("pr_verifycode", new object[] { mobileNo, uCode },false);
                if (strRetValue == "1")
                    flag = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;

        }

        public String insertVolunteers(Volunteer oVolunteerBiz)
        {
            CDL.CoreDataLayer objCoreDataLayer = new CDL.CoreDataLayer();
            Boolean flag = false;
            string strRetValue = string.Empty;
            try
            {
                
                strRetValue = objCoreDataLayer.ExecuteScalar("PR_INSVolunteer", new object[] {oVolunteerBiz.VolunteerName,oVolunteerBiz.MobileNo,oVolunteerBiz.VoterId,oVolunteerBiz.AssemblyId,oVolunteerBiz.CorpId,oVolunteerBiz.WardId,oVolunteerBiz.BoothId,oVolunteerBiz.RefMobileNo }, false);


                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRetValue;
        }

        public DataSet GenerateReport(Volunteer oVolunteerBiz)
        {
            DataSet dsMetadata = null;
            CDL.CoreDataLayer objCoreDataLayer = new CDL.CoreDataLayer();
            try
            {
                dsMetadata = new DataSet();
                dsMetadata = objCoreDataLayer.ExecuteDataSet("pr_generatereport", new object[] {oVolunteerBiz.AssemblyId,oVolunteerBiz.CorpId,oVolunteerBiz.WardId,oVolunteerBiz.BoothId});

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dsMetadata;

        }

        
    }
}