using System.ComponentModel;
using System.Drawing;
using System.IO;
using ImagePasswordManager.Encryption;
using Newtonsoft.Json;
using SimplePasswordManager.Models;

namespace SimplePasswordManager.States
{
    internal static class AuthenticationState
    {
        /// <summary>
        ///     Authentication key for encrypt/decrypt
        /// </summary>
        private static string _authenticationKey => "TG92ZVllYWhNb21BbmRQb3Bz";

        /// <summary>
        ///     The file used for authentication storage
        /// </summary>
        internal static string ImageFilePath { get; set; }

        /// <summary>
        ///     Saves the authentications into the specified file
        /// </summary>
        /// <param name="authenticationModels"></param>
        internal static void Save(BindingList<AuthenticationModel> authenticationModels)
        {
            var encryptedJsonString = CipherExtensions.EncryptString(JsonConvert.SerializeObject(authenticationModels), _authenticationKey);

            Bitmap bitmap;

            using (var stream = new FileStream(ImageFilePath, FileMode.Open, FileAccess.Read))
            {
                bitmap = SteganographyExtensions.CreateNonIndexedImage(Image.FromStream(stream));
            }

            var encryptedImage = SteganographyExtensions.SaveString(encryptedJsonString, bitmap);
            encryptedImage.Save(ImageFilePath);
        }

        /// <summary>
        ///     Determines if any authentication has been stored in a file by checking for any base64 information
        /// </summary>
        /// <returns></returns>
        internal static bool IsAuthenticatedFile()
        {
            var encryptedJsonString = SteganographyExtensions.GetString(
                new Bitmap(Image.FromFile(ImageFilePath)));

            return encryptedJsonString.IsBase64String();
        }

        /// <summary>
        ///     Loads the authentication from a file
        /// </summary>
        /// <returns></returns>
        internal static BindingList<AuthenticationModel> Load()
        {
            var authenticationModels = new BindingList<AuthenticationModel>();

            if (!IsAuthenticatedFile())
            {
                return authenticationModels;
            }

            Bitmap bitmap;

            using (var stream = new FileStream(ImageFilePath, FileMode.Open, FileAccess.Read))
            {
                bitmap = SteganographyExtensions.CreateNonIndexedImage(Image.FromStream(stream));
            }

            var encryptedJsonString = SteganographyExtensions.GetString(new Bitmap(bitmap));
            var decryptedJsonString = CipherExtensions.DecryptString(encryptedJsonString, _authenticationKey);

            authenticationModels = JsonConvert.DeserializeObject<BindingList<AuthenticationModel>>(decryptedJsonString);

            return authenticationModels;
        }
    }
}