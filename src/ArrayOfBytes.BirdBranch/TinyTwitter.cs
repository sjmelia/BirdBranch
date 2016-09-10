using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// Taken from here and adjusted for dotnetcore
/// https://github.com/jmhdez/TinyTwitter
namespace ArrayOfBytes.BirdBranch
{
    public class OAuthInfo
	{
		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public string AccessToken { get; set; }
		public string AccessSecret { get; set; }
	}

	public class TinyTwitter
	{
		private readonly OAuthInfo oauth;

		public TinyTwitter(OAuthInfo oauth)
		{
			this.oauth = oauth;
		}

		public async void UpdateStatus(string message)
		{
			await new RequestBuilder(oauth, "POST", "https://api.twitter.com/1.1/statuses/update.json")
				.AddParameter("status", message)
				.Execute();
		}

        public async void DirectMessageNew(string screenName, string text)
        {
            await new RequestBuilder(oauth, "POST", "https://api.twitter.com/1.1/direct_messages/new.json")
                .AddParameter("screen_name", screenName)
                .AddParameter("text", text)
                .Execute();
        }

		#region RequestBuilder

		public class RequestBuilder
		{
			private const string VERSION = "1.0";
			private const string SIGNATURE_METHOD = "HMAC-SHA1";

			private readonly OAuthInfo oauth;
			private readonly string method;
			private readonly IDictionary<string, string> customParameters;
			private readonly string url;

			public RequestBuilder(OAuthInfo oauth, string method, string url)
			{
				this.oauth = oauth;
				this.method = method;
				this.url = url;
				customParameters = new Dictionary<string, string>();
			}

			public RequestBuilder AddParameter(string name, string value)
			{
				customParameters.Add(name, value.EncodeRFC3986());
				return this;
			}

			public async Task<string> Execute()
			{
				var timespan = GetTimestamp();
				var nonce = CreateNonce();

				var parameters = new Dictionary<string, string>(customParameters);
				AddOAuthParameters(parameters, timespan, nonce);

				var signature = GenerateSignature(parameters);
				var headerValue = GenerateAuthorizationHeaderValue(parameters, signature);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", headerValue);
                if (method == "GET")
                {   
                    return await client.GetStringAsync(GetRequestUrl());
                }
                else if (method == "POST")
                {
                    var content = new StringContent(GetCustomParametersString(), Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = await client.PostAsync(GetRequestUrl(), content);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new NotImplementedException();
                }
			}

			private string GetRequestUrl()
			{
				if (method != "GET" || customParameters.Count == 0)
					return url;

				return string.Format("{0}?{1}", url, GetCustomParametersString());
			}

			private string GetCustomParametersString()
			{
				return customParameters.Select(x => string.Format("{0}={1}", x.Key, x.Value)).Join("&");
			}

			private string GenerateAuthorizationHeaderValue(IEnumerable<KeyValuePair<string, string>> parameters, string signature)
			{
				return new StringBuilder()
					.Append(parameters.Concat(new KeyValuePair<string, string>("oauth_signature", signature))
								.Where(x => x.Key.StartsWith("oauth_"))
								.Select(x => string.Format("{0}=\"{1}\"", x.Key, x.Value.EncodeRFC3986()))
								.Join(","))
					.ToString();
			}

			private string GenerateSignature(IEnumerable<KeyValuePair<string, string>> parameters)
			{
				var dataToSign = new StringBuilder()
					.Append(method).Append("&")
					.Append(url.EncodeRFC3986()).Append("&")
					.Append(parameters
								.OrderBy(x => x.Key)
								.Select(x => string.Format("{0}={1}", x.Key, x.Value))
								.Join("&")
								.EncodeRFC3986());

				var signatureKey = string.Format("{0}&{1}", oauth.ConsumerSecret.EncodeRFC3986(), oauth.AccessSecret.EncodeRFC3986());
				var sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(signatureKey));

				var signatureBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(dataToSign.ToString()));
				return Convert.ToBase64String(signatureBytes);
			}

			private void AddOAuthParameters(IDictionary<string, string> parameters, string timestamp, string nonce)
			{
				parameters.Add("oauth_version", VERSION);
				parameters.Add("oauth_consumer_key", oauth.ConsumerKey);
				parameters.Add("oauth_nonce", nonce);
				parameters.Add("oauth_signature_method", SIGNATURE_METHOD);
				parameters.Add("oauth_timestamp", timestamp);
				parameters.Add("oauth_token", oauth.AccessToken);
			}

			private static string GetTimestamp()
			{
				return ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
			}

			private static string CreateNonce()
			{
				return new Random().Next(0x0000000, 0x7fffffff).ToString("X8");
			}
		}

		#endregion
	}

	public static class TinyTwitterHelperExtensions
	{
		public static string Join<T>(this IEnumerable<T> items, string separator)
		{
			return string.Join(separator, items.ToArray());
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T value)
		{
			return items.Concat(new[] { value });
		}

		public static string EncodeRFC3986(this string value)
		{
			// From Twitterizer http://www.twitterizer.net/

			if (string.IsNullOrEmpty(value))
				return string.Empty;

			var encoded = Uri.EscapeDataString(value);

			return Regex
				.Replace(encoded, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper())
				.Replace("(", "%28")
				.Replace(")", "%29")
				.Replace("$", "%24")
				.Replace("!", "%21")
				.Replace("*", "%2A")
				.Replace("'", "%27")
				.Replace("%7E", "~");
		}
	}
}
