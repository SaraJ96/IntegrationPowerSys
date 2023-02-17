using System;
using FTN.Common;
using FTN.ServiceContracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientUIApp.GDA
{
    public class GDAQuery
    {
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();

        private NetworkModelGDAProxy gdaQueryProxy = null;
        private NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }

                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }


        public GDAQuery()
        {
        }


        #region GDAQueryService

        //added methods
        public List<long> GetAllGIDs(List<ModelCode> modelCodes)
        {
            string message = string.Empty;
            List<long> allGids = new List<long>();
            ModelCode mcForException = ModelCode.CONTROL;
            int iteratorID = 0;

            try
            {
                foreach (ModelCode mc in modelCodes)
                {
                    mcForException = mc;
                    int numberOfResources = 1;
                    int resourcesLeft = 0;

                    List<ModelCode> properties = new List<ModelCode>();
                    properties.Add(ModelCode.IDOBJ_GID);

                    iteratorID = GdaQueryProxy.GetExtentValues(mc, properties);
                    resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorID);



                    while (resourcesLeft > 0)
                    {
                        List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorID);

                        foreach (ResourceDescription rd in rds)
                        {
                            allGids.Add(rd.Id);
                        }

                        resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorID);
                    }

                    GdaQueryProxy.IteratorClose(iteratorID);
                }
            }
            catch (Exception e)
            {
                message = string.Format("[GDAQUERY] GetValues method failed for {0}.\n\t{1}", mcForException, e.Message);
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            }


            return allGids;
        }

        public List<ModelCode> GetAllPropertiesFromDMSType(DMSType type)
        {
            return modelResourcesDesc.GetAllPropertyIds(type);
        }

        public List<ModelCode> GetAllPropertiesFromModelCode(ModelCode modelCode)
        {
            return modelResourcesDesc.GetAllPropertyIds(modelCode);
        }


        public ResourceDescription GetValues(long globalId, List<ModelCode> props)
        {
            string message = "Getting values method started.";
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceError, message);

            XmlTextWriter xmlWriter = null;
            ResourceDescription rd = null;

            try
            {
                short type = ModelCodeHelper.ExtractTypeFromGlobalId(globalId);
                if (props == null)
                {
                    List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds((DMSType)type);
                    rd = GdaQueryProxy.GetValues(globalId, properties);
                }
                else
                {
                    rd = GdaQueryProxy.GetValues(globalId, props);
                }


                xmlWriter = new XmlTextWriter(Config.Instance.ResultDirecotry + "\\GetValues_Results.xml", Encoding.Unicode);
                xmlWriter.Formatting = Formatting.Indented;
                rd.ExportToXml(xmlWriter);
                xmlWriter.Flush();

                message = "Getting values method successfully finished.";
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            }
            catch (Exception e)
            {
                message = string.Format("Getting values method for entered id = {0} failed.\n\t{1}", globalId, e.Message);
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            }
            finally
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                }
            }

            return rd;
        }

        public List<long> GetExtentValues(ModelCode modelCode, List<ModelCode> props)
        {
            string message = "Getting extent values method started.";
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceError, message);

            XmlTextWriter xmlWriter = null;
            int iteratorId = 0;
            List<long> ids = new List<long>();

            try
            {
                int numberOfResources = 2;
                int resourcesLeft = 0;

                if (props == null)
                {
                    List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds(modelCode);
                    iteratorId = GdaQueryProxy.GetExtentValues(modelCode, properties);
                }
                else
                {
                    iteratorId = GdaQueryProxy.GetExtentValues(modelCode, props);
                }

                resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);


                xmlWriter = new XmlTextWriter(Config.Instance.ResultDirecotry + "\\GetExtentValues_Results.xml", Encoding.Unicode);
                xmlWriter.Formatting = Formatting.Indented;

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        ids.Add(rds[i].Id);
                        rds[i].ExportToXml(xmlWriter);
                        xmlWriter.Flush();
                    }

                    resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                GdaQueryProxy.IteratorClose(iteratorId);

                message = "Getting extent values method successfully finished.";
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);

            }
            catch (Exception e)
            {
                message = string.Format("Getting extent values method failed for {0}.\n\t{1}", modelCode, e.Message);
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            }
            finally
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                }
            }

            return ids;
        }

        public List<long> GetRelatedValues(long sourceGlobalId, List<ModelCode> properties, Association association)
        {
            string message = "Getting related values method started.";
            Console.WriteLine(message);
            CommonTrace.WriteTrace(CommonTrace.TraceError, message);

            List<long> resultIds = new List<long>();


            XmlTextWriter xmlWriter = null;
            int numberOfResources = 2;

            try
            {

                int iteratorId = GdaQueryProxy.GetRelatedValues(sourceGlobalId, properties, association);
                int resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);

                xmlWriter = new XmlTextWriter(Config.Instance.ResultDirecotry + "\\GetRelatedValues_Results.xml", Encoding.Unicode);
                xmlWriter.Formatting = Formatting.Indented;

                while (resourcesLeft > 0)
                {
                    List<ResourceDescription> rds = GdaQueryProxy.IteratorNext(numberOfResources, iteratorId);

                    for (int i = 0; i < rds.Count; i++)
                    {
                        resultIds.Add(rds[i].Id);
                        rds[i].ExportToXml(xmlWriter);
                        xmlWriter.Flush();
                    }

                    resourcesLeft = GdaQueryProxy.IteratorResourcesLeft(iteratorId);
                }

                GdaQueryProxy.IteratorClose(iteratorId);

                message = "Getting related values method successfully finished.";
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
            }
            catch (Exception e)
            {
                message = string.Format("Getting related values method  failed for sourceGlobalId = {0} and association (propertyId = {1}, type = {2}). Reason: {3}", sourceGlobalId, association.PropertyId, association.Type, e.Message);
                Console.WriteLine(message);
                CommonTrace.WriteTrace(CommonTrace.TraceError, message);
                throw new Exception($"{e.Message}");
            }
            finally
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                }
            }

            return resultIds;
        }

        #endregion GDAQueryService

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}