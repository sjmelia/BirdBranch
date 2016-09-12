Bird Branch - dotnetcore logger for Twitter
===========================================
[![Build Status](https://travis-ci.org/sjmelia/TeensyTwitter.svg)](https://travis-ci.org/sjmelia/birdbranch)
[![nuget](https://img.shields.io/badge/nuget-v5.3.0-blue.svg)](https://www.nuget.org/packages/ArrayOfBytes.BirdBranch)

Who would do this?
------------------
Yes, it's a bit crazy; but useful for infrequently logging stuff to an easily accessible endpoint.

To use
------
Set yourself up a Twitter app to get the various keys; then, for the default asp.net core template;

In your `Startup.cs`:
``` C#
using ArrayOfBytes.BirdBranch;
using ArrayOfBytes.OAuth.Client;

...

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
	// ... other service configuration.

	// Add a Twitter status logger, including an (optional) filter so that only messages
	// with the "TwitterCategory" category are sent. (Reducing amount of logging)
	loggerFactory.AddTwitterStatus(new OAuthConfig()
				{
					ConsumerKey = "<consumer key>",
					ConsumerSecret = "<consumer secret>",
					AccessToken = "<access token>",
					AccessSecret = "<access secret>"
	            },
				(o, ll) => o == "TwitterCategory")

	// ... other service configuration
}
```

Getting hold of an `ILoggerFactory` and calling `CreateLogger` will now let you log straight into your timeline. 
You can also direct message a user from your selected account, using the `AddTwitterDirectMessage` overload.
Both functions accept a filter as is usual for logging functions - and it's highly advisable to use it!