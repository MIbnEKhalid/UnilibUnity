using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
public class versioncheck : MonoBehaviour
{
    static string GithubReleaseVersionNumber;
    public GameObject VersionNews;
    // Start is called before the first frame update
    async void Start()
    { 
        CheckVersion();
    }

    // Update is called once per frame
    async void CheckVersion()
    {
        string githubVersion = await RetrivrVersionNumberFromGithub();
        string appVersion = Application.version;

        if (githubVersion != null)
        {
            if (githubVersion != appVersion)
            {
                VersionNews.SetActive(true);
                Debug.Log($"A new version is available: {githubVersion}. Current version: {appVersion}");
            }
            else
            {
                Debug.Log("You are using the latest version.");
            }
        }
        else
        {
            Debug.Log("Failed to retrieve the version from GitHub.");
        }
    }

    async Task<string> RetrivrVersionNumberFromGithub()
    {
        string repoOwner = "MIbnEKhalid";
        string repoName = "UnilibUnity";
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";

        using (HttpClient client = new HttpClient())
        {
            // Add headers
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            try
            {
                // Fetch response
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Parse JSON
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject release = JObject.Parse(responseBody);

                // Extract details
                string releaseName = release["name"]?.ToString();
                string tagName = release["tag_name"]?.ToString();
                string releaseUrl = release["html_url"]?.ToString();

                //Debug.Log($"Latest Release: {releaseName}");
                //Debug.Log($"Tag: {tagName}");
                //Debug.Log($"URL: {releaseUrl}");

                string pattern = @"\[(.*?)\]";
                Match match = Regex.Match(releaseName, pattern);

                if (match.Success)
                {
                    string versionNumber = match.Groups[1].Value;
                    GithubReleaseVersionNumber = versionNumber;
                //    Debug.Log($"Extracted Version Number: {GithubReleaseVersionNumber}");
                    return GithubReleaseVersionNumber;
                }
                else
                {
                    Debug.Log("No version number found in the string.");
                }

            }
            catch (HttpRequestException e)
            {
                Debug.Log($"Error: {e.Message}");
            }
        }
        return null;
    }
}
