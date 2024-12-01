using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class versioncheck : MonoBehaviour
{
    public GameObject VersionNews;

    public TMP_InputField detailText;
    public TMP_Text versionText;
    async void Start()
    {
        await CheckVersion();
    }

    // Convert Markdown to TextMeshPro Rich Text

    string ConvertMarkdownToRichText(string markdown)
    {
        // Remove comments between <!-- and -->
        markdown = Regex.Replace(markdown, @"<!--.*?-->", string.Empty, RegexOptions.Singleline);

        // Convert bold text (e.g., **bold** or __bold__)
        markdown = Regex.Replace(markdown, @"(\*\*|__)(.*?)\1", "<b>$2</b>");

        // Convert italic text (e.g., *italic* or _italic_)
        markdown = Regex.Replace(markdown, @"(\*|_)(.*?)\1", "<i>$2</i>");

        // Convert links (e.g., [link](url))
        markdown = Regex.Replace(markdown, @"\[(.*?)\]\((.*?)\)", "<color=#0000EE><u>$1</u></color>");

        // Convert headers (e.g., # Header -> <size=24>Header</size>)
        markdown = Regex.Replace(markdown, @"^(#{1,6})\s*(.*?)$", match =>
        {
            int headerLevel = match.Groups[1].Length;
            int fontSize = 24 - (headerLevel * 2);
            return $"<size={fontSize}>{match.Groups[2].Value}</size>";
        }, RegexOptions.Multiline);

        // Convert newlines to line breaks
        markdown = markdown.Replace("\n", "<br>");

        return markdown;
    }

    async Task CheckVersion()
    {
        string githubVersion = await RetrieveVersionNumberFromGithub();
        string appVersion = Application.version;

        if (!string.IsNullOrEmpty(githubVersion))
        {
            try
            {
                Version appVer = new Version(appVersion);
                Version gitVer = new Version(githubVersion);

                if (gitVer > appVer)
                {
                    VersionNews.SetActive(true);
                    Debug.Log($"A new version is available: {githubVersion}. Current version: {appVersion}");
                }
                else
                {
                    Debug.Log("You are using the latest version.");
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error parsing version numbers: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("Failed to retrieve the version from GitHub.");
        }
    }

    async Task<string> RetrieveVersionNumberFromGithub()
    {
        string url = "https://api.github.com/repos/MIbnEKhalid/UnilibUnity/releases/latest";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "CSharp-App");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject release = JObject.Parse(responseBody);

                string releaseName = release["name"]?.ToString();
                Debug.Log($"Latest Release: {releaseName}");
                versionText.text = releaseName;
                string releaseDate = release["published_at"]?.ToString();
                Debug.Log(releaseDate);

                string releaseDetails = release["body"]?.ToString();
                Debug.Log("releaseDetails: " + releaseDetails);
                detailText.text = ConvertMarkdownToRichText(releaseDetails); ;
;
                string pattern = @"\[(.*?)\]";
                Match match = Regex.Match(releaseName, pattern);

                if (match.Success)
                {
                    return match.Groups[1].Value;
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
