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
add a `using ArrayOfBytes.BirdBranch` to get the extension methods and add the following in the
`Configure` method of `Startup.cs` to log straight into your account's timeline:

`loggerFactory.AddTwitterStatus(new ArrayOfBytes.BirdBranch.OAuthInfo()
            {
                ConsumerKey = "<consumer key>",
                ConsumerSecret = "<consumer secret>",
                AccessToken = "<access token>",
                AccessSecret = "<access secret>"
            });`

You can also direct message a user from your selected account, using the `AddTwitterDirectMessage` overload.
Both functions accept a filter as is usual for logging functions.