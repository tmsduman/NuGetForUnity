using NugetForUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NugetForUnity.V3
{
    public class NugetPackageSourceV3 : NugetPackageSource
    {
        #region IndexResponse
        private V3IndexResponse localIndexResponse;
        internal V3IndexResponse IndexResponse
        {
            get
            {
                if (this.localIndexResponse == null)
                {
                    string json = GetDataFromUrlAsString(ExpandedPath, UserName, ExpandedPassword);
                    // We need a custom deserialization because JsonUtility.FromJson did not support @ types
                    json = json.Replace("\"@id\"", "\"id\"");
                    json = json.Replace("\"@type\"", "\"type\"");
                    this.localIndexResponse = JsonUtility.FromJson<V3IndexResponse>(json);
                }
                return this.localIndexResponse;
            }
        }
        #endregion

        private string queryUrl => this.IndexResponse?.resources.FirstOrDefault(x => x.type.Equals("SearchQueryService"))?.id ?? string.Empty;
        private string metaDataUrl => this.IndexResponse?.resources.FirstOrDefault(x => x.type.Equals("RegistrationsBaseUrl"))?.id ?? string.Empty;

        public NugetPackageSourceV3(string name, string path) : base(name, path)
        {
        }

        protected override List<NugetPackage> FindPackagesByIdOnline(NugetPackageIdentifier package)
        {
            return SearchOnline(package.Id);
        }

        protected override NugetPackage GetSpecificPackageOnline(NugetPackageIdentifier package)
        {            
            string url = string.Format("{0}/{1}/{2}.json", metaDataUrl, package.Id, package.Version);

            try
            {
                return GetPackagesFromUrl(url, UserName, ExpandedPassword).First();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Unable to retrieve package from {0}\n{1}", url, e.ToString());
                return null;
            }
        }

        protected override List<NugetPackage> RequestPackagesFromUrl(string url, string username, string password)
        {
            List<NugetPackage> packages = new List<NugetPackage>();

            string json = GetDataFromUrlAsString(url, username, password);
            V3Response v3Response = JsonUtility.FromJson<V3Response>(json);
            if (v3Response?.data?.Count > 0)
            {
                foreach (V3ResponseData dataEntry in v3Response.data)
                {
                    NugetPackage v3Package = new NugetPackage
                    {
                        Authors = string.Join(",", dataEntry.authors),
                        Id = dataEntry.id,
                        Title = dataEntry.title,
                        Summary = dataEntry.summary,
                        DownloadCount = dataEntry.totalDownloads,
                        Version = dataEntry.version,
                        DownloadUrl = string.Empty,
                        Description = dataEntry.description,
                        ProjectUrl = dataEntry.projectUrl,
                        LicenseUrl = dataEntry.licenseUrl,
                        Icon = !string.IsNullOrEmpty(dataEntry.iconUrl) ? NugetHelper.DownloadImage(dataEntry.iconUrl) : null
                };

                    packages.Add(v3Package);
                }
            }
            else
            {
                V3MetaResponse v3MetaResponse = JsonUtility.FromJson<V3MetaResponse>(json);
                if (v3MetaResponse?.catalogEntry != null)
                {
                    NugetPackage v3MetaPackage = new NugetPackage
                    {
                        Id = v3MetaResponse.catalogEntry.id,
                        Authors = string.Join(",", v3MetaResponse.catalogEntry.authors),
                        Version = v3MetaResponse.catalogEntry.version,
                        Summary = v3MetaResponse.catalogEntry.summary,
                        DownloadUrl = v3MetaResponse.catalogEntry.packageContent
                    };

                    packages.Add(v3MetaPackage);
                }
            }

            return packages;
        }

        private string GetDataFromUrlAsString(string url, string username, string password)
        {
            string responseData = string.Empty;

            using (Stream responseStream = GetDataFromUrl(url, username, password))
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    responseData = streamReader.ReadToEnd();
                }
            }

            return responseData;
        }

        protected override List<NugetPackage> SearchOnline(string searchTerm = "", bool includeAllVersions = false, bool includePrerelease = false, int numberToGet = 15, int numberToSkip = 0)
        {
            string url = queryUrl;

            // call the search method
            url += "?";

            // apply the search term
            url += string.Format("q={0}&", searchTerm);

            // skip a certain number of entries
            url += string.Format("skip={0}&", numberToSkip);

            // show a certain number of entries
            url += string.Format("take={0}&", numberToGet);

            // should we include prerelease packages?
            url += string.Format("prerelease={0}", includePrerelease.ToString().ToLower());

            try
            {
                return GetPackagesFromUrl(url, UserName, ExpandedPassword);
            }
            catch (System.Exception e)
            {
                NugetHelper.LogVerbose("Unable to retrieve package list from {0}\n{1}", url, e.ToString());
                return new List<NugetPackage>();
            }
        }
    }
}
