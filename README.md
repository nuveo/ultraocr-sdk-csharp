# UltraOCR SDK C#

UltraOCR SDK for C#.

[UltraOCR](https://ultraocr.com.br/) is a platform that assists in the document analysis process with AI.

For more details about the system, types of documents, routes and params, access our [documentation](https://docs.nuveo.ai/ocr/v2/).

## Instalation

First of all, you must install this package with nuget:

```
dotnet add package Ultraocr --version 1.0.0
```

Then you must import the UltraOCR SDK in your code with:

```csharp
using Ultraocr;
```

## Step by step

### First step - Client Creation and Authentication

With the UltraOCR SDK installed and imported, the first step is create the Client and authenticate, you have two ways to do it.

The first one, you can do it in two steps with:

```csharp
Client client = new();
await client.Authenticate("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET", 60);
```

The third argument on `Authenticate` function is `expires`, a long between `1` and `1440`, the Token time expiration in minutes.

Another way is creating the client with `AutoRefresh` option. As example:

```csharp
Client client = new("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET", 60);
```

or:

```csharp
Client client = new();
client.SetAutoRefresh("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET", 60);
```

The Client have following allowed setters:

- `SetAutoRefresh`: Set client to be auto refreshed.
- `SetAuthBaseUrl`: Change the base url to authenticate (Default UltraOCR url).
- `SetBaseUrl`: Change the base url to send documents (Default UltraOCR url).
- `SetTimeout`: Change the pooling timeout in seconds (Default 30).
- `SetInterval`: Change the pooling interval in seconds (Default 1).
- `SetHttpClient`: Change http client to do requests on UltraOCR (Default newHttpClient()).

### Second step - Send Documents

With everything set up, you can send documents:

```csharp
Dictionary<string,string> parameters = [];
Dictionary<string,Object> metadata = [];
Dictionary<string,Object>[] batchMetadata = [];
await client.SendJob("SERVICE", "FILE_PATH", metadata, parameters); // Simple job
await client.SendBatch("SERVICE", "FILE_PATH", batchMetadata, parameters); // Simple batch
await client.SendJobBase64("SERVICE", Encoding.ASCII.GetBytes("BASE64_DATA"), metadata, parameters); // Job in base64
await client.SendBatchBase64("SERVICE", Encoding.ASCII.GetBytes("BASE64_DATA"), batchMetadata, parameters); // Batch in base64
await client.SendJobSingleStep("SERVICE", "BASE64_DATA", metadata, parameters); // Job in base64, faster, but with limits
```

Send batch response example:

```csharp
{
  Id: "0ujsszwN8NRY24YaXiTIE2VWDTS",
  StatusUrl: "https://ultraocr.apis.nuveo.ai/v2/ocr/batch/status/0ujsszwN8NRY24YaXiTIE2VWDTS"
}
```

Send job response example:

```csharp
{
  Id: "0ujsszwN8NRY24YaXiTIE2VWDTS",
  StatusUrl: "https://ultraocr.apis.nuveo.ai/v2/ocr/job/result/0ujsszwN8NRY24YaXiTIE2VWDTS"
}
```

For jobs, to send facematch file (if requested on query params or using facematch service) or extra file (if requested on query params) to send job with document back side you must pass the files info after main document file.

Examples using CNH service and sending facematch and/or extra files:

```csharp
// jobs with only extra document
parameters.TryAdd("extra-document", "true");
await client.SendJob("cnh", "FILE_PATH", "", "EXTRA_FILE_PATH", metadata, parameters);
await client.SendJobBase64("cnh", Encoding.ASCII.GetBytes("BASE64_DATA"), [], Encoding.ASCII.GetBytes("EXTRA_BASE64_DATA"), metadata, parameters);
await client.SendJobSingleStep("cnh", "BASE64_DATA", "", "EXTRA_BASE64_DATA", metadata, parameters);

// jobs with facematch and extra document
parameters.TryAdd("facematch", "true");
await client.SendJob("cnh", "FILE_PATH", "FACEMATCH_FILE_PATH", "EXTRA_FILE_PATH", metadata, parameters);
await client.SendJobBase64("cnh", Encoding.ASCII.GetBytes("BASE64_DATA"), Encoding.ASCII.GetBytes("FACEMATCH_BASE64_DATA"), Encoding.ASCII.GetBytes("EXTRA_BASE64_DATA"), metadata, parameters);
await client.SendJobSingleStep("cnh", "BASE64_DATA", "FACEMATCH_BASE64_DATA", "EXTRA_BASE64_DATA", metadata, parameters);

// jobs with only extra document
parameters.TryAdd("extra-document", "false");
await client.SendJob("cnh", "FILE_PATH", "FACEMATCH_FILE_PATH", "", metadata, parameters);
await client.SendJobBase64("cnh", Encoding.ASCII.GetBytes("BASE64_DATA"), Encoding.ASCII.GetBytes("FACEMATCH_BASE64_DATA"), [], metadata, parameters);
await client.SendJobSingleStep("cnh", "BASE64_DATA", "FACEMATCH_BASE64_DATA", "", metadata, parameters);
```

Alternatively, you can request the signed url directly, without any utility, but you will must to upload the document manually. Example:

```csharp
using Ultraocr.Enums;

var response = await client.GenerateSignedUrl("SERVICE", Resource.Job, metadata, parameters); // Request job
Map<String, String> urls = response.getUrls();
var urls = response.Urls;
var url = urls["document"];
byte[] file = await File.ReadAllBytesAsync(filePath);
// PUT to url with file on body

var response = await client.GenerateSignedUrl("SERVICE", Resource.Batch, batchMetadata, parameters); // Request batch
var urls = response.Urls;
var url = urls["document"];
byte[] file = await File.ReadAllBytesAsync(filePath);
// PUT to url with file on body
```

Example of response from `GenerateSignedUrl` with facematch and extra files:

```csharp
{
  Exp: "60000",
  Id: "0ujsszwN8NRY24YaXiTIE2VWDTS",
  StatusUrl: "https://ultraocr.apis.nuveo.ai/v2/ocr/batch/status/0ujsszwN8NRY24YaXiTIE2VWDTS",
  Urls: {
    "document": "https://presignedurldemo.s3.eu-west-2.amazonaws.com/image.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAJJWZ7B6WCRGMKFGQ%2F20180210%2Feu-west-2%2Fs3%2Faws4_request&X-Amz-Date=20180210T171315Z&X-Amz-Expires=1800&X-Amz-Signature=12b74b0788aa036bc7c3d03b3f20c61f1f91cc9ad8873e3314255dc479a25351&X-Amz-SignedHeaders=host",
    "selfie": "https://presignedurldemo.s3.eu-west-2.amazonaws.com/image.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAJJWZ7B6WCRGMKFGQ%2F20180210%2Feu-west-2%2Fs3%2Faws4_request&X-Amz-Date=20180210T171315Z&X-Amz-Expires=1800&X-Amz-Signature=12b74b0788aa036bc7c3d03b3f20c61f1f91cc9ad8873e3314255dc479a25351&X-Amz-SignedHeaders=host",
    "extra_document": "https://presignedurldemo.s3.eu-west-2.amazonaws.com/image.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAJJWZ7B6WCRGMKFGQ%2F20180210%2Feu-west-2%2Fs3%2Faws4_request&X-Amz-Date=20180210T171315Z&X-Amz-Expires=1800&X-Amz-Signature=12b74b0788aa036bc7c3d03b3f20c61f1f91cc9ad8873e3314255dc479a25351&X-Amz-SignedHeaders=host"
  }
}
```

### Third step - Get Result

With the job or batch id, you can get the job result or batch status with:

```csharp
var response = await client.GetBatchStatus("BATCH_ID"); // Batches
var jobResponse = await client.GetJobResult("JOB_ID", "JOB_ID"); // Simple jobs
var jobResponse2 = await client.GetJobResult("BATCH_ID", "JOB_ID"); // Jobs belonging to batches

var batchResult = await client.GetBatchResult("BATCH_ID"); // Get batch jobs result as array
var storage = await client.GetBatchResultStorage("BATCH_ID", parameters); // Get batch jobs result in a file

// More details about job and batch
var batchInfo = await client.GetBatchInfo("BATCH_ID"); // Batches info (without jobs info)
var jobInfo = await client.GetJobInfo("JOB_ID"); // Jobs info (single jobs only)
```

Alternatively, you can use a utily `WaitForJobDone` or `WaitForBatchDone`:

```csharp
var response = await client.WaitForBatchDone("BATCH_ID", true); // Batches, ends when the batch and all it jobs are finished
var response2 = await client.WaitForBatchDone("BATCH_ID", false); // Batches, ends when the batch is finished
var jobResponse = await client.WaitForJobDone("JOB_ID", "JOB_ID"); // Simple jobs
var jobResponse2 = await client.WaitForJobDone("BATCH_ID", "JOB_ID"); // Jobs belonging to batches
```

Batch status example:

```csharp
{
  BatchKsuid: "2AwrSd7bxEMbPrQ5jZHGDzQ4qL3",
  CreatedAt: "2022-06-22T20:58:09Z",
  Jobs: [
    {
      "created_at": "2022-06-22T20:58:09Z",
      "job_ksuid": "0ujsszwN8NRY24YaXiTIE2VWDTS",
      "result_url": "https://ultraocr.apis.nuveo.ai/v2/ocr/job/result/2AwrSd7bxEMbPrQ5jZHGDzQ4qL3/0ujsszwN8NRY24YaXiTIE2VWDTS",
      "status": "processing"
    }
  ],
  Service: "cnh",
  Status: "done"
}
```

Job result example:

```csharp
{
  Created_at: "2022-06-22T20:58:09Z",
  JobKsuid: "2AwrSd7bxEMbPrQ5jZHGDzQ4qL3",
  Result: {
    "Time": "7.45",
    "Document": [
      {
        "Page": 1,
        "Data": {
          "DocumentType": {
            "conf": 99,
            "value": "CNH"
          }
        }
      }
    ]
  },
  Service: "idtypification",
  Status: "done"
}
```

### Simplified way

You can do all steps in a simplified way, with `CreateAndWaitJob` or `CreateAndWaitBatch` utilities:

```csharp
using Ultraocr;

Client client = new("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET", 60);
var response = await client.CreateAndWaitJob("SERVICE", "FILE_PATH", metadata, parameters); // simple job
var response2 = await client.CreateAndWaitJob("cnh", "FILE_PATH", "FACEMATCH_FILE_PATH", "EXTRA_FILE_PATH", metadata, parameters); // job with facematch and extra file
```

Or:

```csharp
using Ultraocr;

Client client = new("YOUR_CLIENT_ID", "YOUR_CLIENT_SECRET", 60);
var response = await client.CreateAndWaitBatch("SERVICE", "FILE_PATH", batchMetadata, parameters, false);
```

The `CreateAndWaitJob` has the `SendJob` arguments and `GetJobResult` response, while the `CreateAndWaitBatch` has the `SendBatch` arguments with the addition of `waitJobs` as last parameter and has the `GetBatchStatus` response.

### Get many results

You can get all jobs in a given interval by calling `GetJobs` utility:

```csharp
var jobs = await client.GetJobs("START_DATE", "END_DATE") // Dates in YYYY-MM-DD format
```

Results:

```csharp
[
  {
    CreatedAt: "2022-06-22T20:58:09Z",
    JobKsuid: "2AwrSd7bxEMbPrQ5jZHGDzQ4qL3",
    Result: {
      "Time": "7.45",
      "Document": [
        {
          "Page": 1,
          "Data": {
            "DocumentType": {
              "conf": 99,
              "value": "CNH"
            }
          }
        }
      ]
    },
    Service: "idtypification",
    Status: "done"
  },
  {
    CreatedAt: "2022-06-22T20:59:09Z",
    JobKsuid: "2AwrSd7bxEMbPrQ5jZHGDzQ4qL4",
    Result: {
      "Time": "8.45",
      "Document": [
        {
          "Page": 1,
          "Data": {
            "DocumentType": {
              "conf": 99,
              "value": "CNH"
            }
          }
        }
      ]
    },
    Service: "cnh",
    Status: "done"
  },
  {
    CreatedAt: "2022-06-22T20:59:39Z",
    JobKsuid: "2AwrSd7bxEMbPrQ5jZHGDzQ4qL5",
    Service: "cnh",
    Status: "processing"
  }
]
```
