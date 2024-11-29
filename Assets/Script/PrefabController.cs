using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking; // For UnityWebRequest
using System.Collections;
using System.IO;

public class PrefabController : MonoBehaviour
{
    public TMP_Text titleText;  // Assign the TextMeshPro component
    public Image iconImage;     // Assign the Image component
    public Button urlButton;    // Assign the Button for the URL

    // Variable to store the current image URL for checking
    private string cachedImagePath;

    // Function to initialize the prefab with data
    public void Initialize(string title, string imageUrl, string url)
    {
        Debug.Log("Initializing Prefab with title: " + title + ", imageUrl: " + imageUrl + ", url: " + url);
        titleText.text = title;

        // Store the image URL for cache checking
        cachedImagePath = imageUrl;

        // Check if the image is cached and valid
        if (IsImageCached(imageUrl))
        {
            Debug.Log("Image is cached. Loading from cache.");
            // Load image from cache
            LoadCachedImage();
        }
        else
        {
            Debug.Log("Image is not cached. Downloading image.");
            // Start downloading the image
            StartCoroutine(DownloadAndSetImage(imageUrl));
        }

        // Add listener to the button for opening the URL
        urlButton.onClick.RemoveAllListeners(); // Clear previous listeners
        urlButton.onClick.AddListener(() => Application.OpenURL(url));
    }

    // Check if the image URL matches the cached image
    private bool IsImageCached(string imageUrl)
    {
        string cachedFilePath = GetCacheFilePath(imageUrl);
        Debug.Log("Checking if image is cached at: " + cachedFilePath);

        // Check if cached image exists and URL is the same
        if (File.Exists(cachedFilePath))
        {
            string cachedUrl = File.ReadAllText(cachedFilePath + ".url"); // Store the URL separately
            bool isCached = cachedUrl == imageUrl;
            Debug.Log("Image cached status: " + isCached);
            return isCached;
        }

        Debug.Log("Image is not cached.");
        return false;
    }

    // Load the cached image
    private void LoadCachedImage()
    {
        string cachedFilePath = GetCacheFilePath(cachedImagePath);
        Debug.Log("Loading cached image from: " + cachedFilePath);

        if (File.Exists(cachedFilePath))
        {
            byte[] imageData = File.ReadAllBytes(cachedFilePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData); // Load image data
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            iconImage.sprite = sprite;
            Debug.Log("Cached image loaded successfully.");
        }
        else
        {
            Debug.LogError("Cached image not found at: " + cachedFilePath);
        }
    }

    private IEnumerator DownloadAndSetImage(string imageUrl)
    {
        Debug.Log("Downloading image from URL: " + imageUrl);
        // Download the image from the URL
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading image: {webRequest.error}");
                yield break;
            }

            // Get the downloaded texture
            Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

            // Convert the texture to a sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Assign the sprite to the Image component
            iconImage.sprite = sprite;
            Debug.Log("Image downloaded and set successfully.");

            // Cache the image
            CacheImage(imageUrl, texture);
        }
    }

    // Save the image and URL locally
    private void CacheImage(string imageUrl, Texture2D texture)
    {
        string filePath = GetCacheFilePath(imageUrl);
        Debug.Log("Caching image at: " + filePath);

        // Save the image as a file
        byte[] imageData = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, imageData);

        // Save the URL in a separate file to check if the image is still valid
        File.WriteAllText(filePath + ".url", imageUrl);
        Debug.Log("Image cached successfully.");
    }

    // Get a file path for the cached image based on the URL
    private string GetCacheFilePath(string imageUrl)
    {
        // Ensure the imageUrl is not null
        if (string.IsNullOrEmpty(imageUrl))
        {
            Debug.LogError("Image URL is null or empty.");
            return string.Empty;
        }

        // Use the hash of the image URL to create a unique file path
        string fileName = imageUrl.GetHashCode().ToString();
        return Path.Combine(Application.persistentDataPath, fileName + ".png");
    }
}
