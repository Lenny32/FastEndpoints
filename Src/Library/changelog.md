---

## ✨ Looking For Sponsors ✨

FastEndpoints needs sponsorship to [sustain the project](https://github.com/FastEndpoints/FastEndpoints/issues/449). Please help out if you can.

---

[//]: # (<details><summary>title text</summary></details>)

## New 🎉

<details><summary>Fluent endpoint base class picker</summary>

A fluent endpoint base class picker similar to `Ardalis.ApiEndpoints` has been added, with which you can pick and choose the DTO and Mapper types you'd like to use in a fluent manner.

```csharp
sealed class MyEndpoint : Ep.Req<MyRequest>.Res<MyResponse>.Map<MyMapper>
{
    ...
}

sealed class MyEndpoint : Ep.Req<MyRequest>.NoRes.Map<MyMapper>
{
    ...
}

sealed class MyEndpoint : Ep.NoReq.Res<MyResponse>
{
    ...
}
```

</details>

<details><summary>Job Queuing support for Commands that return a result</summary>

Command that return a results `ICommand<TResult>` can now be queued up as jobs. The result of a job can be retrieved via the **JobTracker** using its **Tracking Id**.

```csharp
// queue the command as a job and retrieve the tracking id 
var trackingId = new MyCommand { ... }.QueueJobAsync();

// retrieve the result of the command using the tracking id
var result = await JobTracker<MyCommand>.GetJobResultAsync<MyResult>(trackingId);
```

[Click here](https://fast-endpoints.com/docs/job-queues#jobs-with-results) to read the documentation for this feature.

</details>

<details><summary>Transform FluentValidation error property names with 'JsonPropertyNamingPolicy'</summary>

A new configuration setting has been introduced so that deeply nested request DTO property names can be transformed to the correct case using the `JsonPropertyNamingPolicy` of the application.

```csharp
app.UseFastEndpoints(c => c.Validation.UsePropertyNamingPolicy = true)
```

The setting is turned on by default and can be turned off by setting the above boolean to `false`.

</details>

<details><summary>Skip anti-forgery checks for certain requests</summary>

The anti-forgery middleware will now accept a filter/predicate which can be used to skip processing certain requests if a given condition is matched. If the function returns true for a certain request, that request will be skipped by the anti-forgery middleware.

```csharp
.UseAntiforgeryFE(httpCtx => !httpCtx.Request.Headers.Origin.IsNullOrEmpty())
```

</details>

## Improvements 🚀

<details><summary>Make Pre/Post Processor Context's 'Request' property nullable</summary>

Since there are certain edge cases where the `Request` property can be `null` such as when STJ receives invalid JSON input from the client and fails to successfully deserialize the content, the pre/post processors would be executed (even in those cases) where the pre/post processor context's `Request` property would be null. This change would allow the compiler to remind you to check for null if the `Request` property is accessed from pre/post processors.

</details>

<details><summary>Preliminary support for .NET 9.0</summary>

Almost everything works with .NET 9 except for source generation. Full .NET 9 support will be available at the first FE release after .NET 9 final comes out.

</details>

## Fixes 🪲

<details><summary>Nullable 'IFormFile' handling issue with 'HttpClient' extensions</summary>

The `HttpClient` extensions for integration testing was not correctly handling nullable `IFormFile` properties in request DTOs when automatically converting them to form fields.

</details>

<details><summary>Swagger processor issue with virtual path routes</summary>

The swagger processor was not correctly handling routes if it starts with a `~/` (virtual path that refers to the root directory of the web application).

</details>

<details><summary>Remove unreferenced schema from generated swagger document</summary>

When a request DTO has a property that's annotated with a `[FromBody]` attribute, the parent schema was left in the swagger document components section as an unreferenced schema. These orphaned schema will no longer be present in the generated swagger spec.

</details>

<details><summary>Swagger request example issue with properties annotated with [FromBody]</summary>

Xml docs based example values were not correctly picked up for properties annotated with a `[FromBody]` attribute, which resulted in a default sample request example being set in Swagger UI.

</details>

[//]: # (## Minor Breaking Changes ⚠️)