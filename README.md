# dotnet-mesh-client

[![CI/CD Pull Request](https://github.com/nhs-england-tools/repository-template/actions/workflows/cicd-1-pull-request.yaml/badge.svg)](https://github.com/nhs-england-tools/repository-template/actions/workflows/cicd-1-pull-request.yaml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=repository-template&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=repository-template)

A dotnet client for accessing the [NHS MESH API](https://digital.nhs.uk/developer/api-catalogue/message-exchange-for-social-care-and-health-api#api-description__end-to-end-process-to-integrate-with-mesh-api)

## Table of Contents

- [dotnet mesh client](dotnet-mesh-client)
  - [Table of Contents](#table-of-contents)
  - [Setup](#setup)
    - [Prerequisites](#prerequisites)
    - [Configuration](#configuration)
  - [Usage](#usage)
    - [Mesh Operation Service](#mesh-operation-service)
    - [Mesh Inbox Service](#mesh-inbox-service)
    - [Mesh Outbox Service](#mesh-outbox-service)
    - [Testing](#testing)

  - [Licence](#licence)

## Setup

### Prerequisites

Currently this is not published to any nuget repository.

To use this package within your dotnet solution we suggest using [git submodules](https://git-scm.com/book/en/v2/Git-Tools-Submodules).

to pull down the repository using git submodules the below command `git submodule add https://github.com/NHSDigital/dotnet-mesh-client` in the directory you wish to pull down the solution into.

the `NHS.Mesh.Client.csproj` should then be added to the solution file.
then the client can be added as a project reference to any project which requires it.

### Configuration

The client needs to be registered as a service for Dependency Injection this can be done by using the below code:

the parameters will need to be updated for your specific mailboxes and environments.
the certificate will need to be converted or created as a `.pfx` file that stores both the certificate and the private key.
the serverSideCertificateCollection should be populated with the certificates of the MESH side server, This is to ensure the host connected to is the expected host.

```c#
        services.AddMeshClient(_ => _.MeshApiBaseUrl = 'MESHURL')
            .AddMailbox("MYMAILBOX",new NHS.MESH.Client.Configuration.MailboxConfiguration
            {
                Password = "Password",
                SharedKey = "SHAREDKEY",
                Cert = new X509Certificate2("path to .pfx file","PFX File password"),
                serverSideCertCollection = new X509Certificate2Collection()
            })
            .Build();
```

Multiple mailboxes can be added by including more `.AddMailbox` methods to the builder and will be resolved when calling the various functions depending on the mailboxId passed to the function.

## Usage

To use all of the functions the needed service class will need to be injected in to the class which requires them as below
To use functions in the mesh Operation Service this needs to injected in the code as below:

```c#
  public class ExampleService
  {
      private readonly IMeshOperationService _meshOperationService

      public ExampleService(IMeshOperationService meshOperationService)
      {
          _meshOperationService = meshOperationService;
      }

      // methods in ExampleService that execute operation service methods.
  }
```

The return type from these functions a `MeshResponse<T>` where T is the successful response data type.
This response also contains an `IsSuccessful` flag which will indicate is the call to MESH returned a successful response.
If the response is unsuccessful the `Error` property will contain a `APIErrorResponse` object that will have further information.
Otherwise the response data will be within the `Response` property.

### Mesh Operation Service

To use this inject `IMeshOperationService` class as shown in [Usage](#usage).

#### Handshake / Validate a mailbox

Use this endpoint to check that MESH can be reached and that the authentication you are using is correct. This endpoint only needs to be called once every 24 hours. This endpoint updates the details of the connection history held for your mailbox and is similar to a keep-alive or ping message, in that it allows monitoring on the Spine to be aware of the active use of a mailbox despite a lack of traffic.

to implement call the

```c#
    var result = await _meshOperationService.MeshHandshakeAsync(mailboxId);
```

This will return the Mailbox Id.

### Mesh Inbox Service

To use this inject `IMeshInboxService` class as shown in [Usage](#usage).
This class contains methods used for receiving messages from MESH.

#### Check an Inbox

Returns the message identifier of messages in the mailbox inbox ready for download.
to implement call the below:

```c#
    var result = await _meshInboxService.GetMessagesAsync(mailboxId);
```

this will return a list of MessageIds that are ready to download.

#### Get Message By Id

Retrieves a message based on the message identifier obtained from the 'GetMessagesAsync' method.
Note this will not retrieve chunked messages.

```c#
    var result = await _meshInboxService.GetMessageByIdAsync(mailboxId, messageId);
```

The response to this will return a `GetMeshResponse` Object which will contain a `FileAttachment` & `MessageMetaData`

#### Get Chunked Message By Id

Retrieves a chunked message based on the message identifier obtained from the 'GetMessagesAsync' method.

```c#
    var result = await _meshInboxService.GetChunkedMessageByIdAsync(mailboxId, messageId);
```

The response to this will return a `GetMeshResponse` Object which will contain a `List<FileAttachment>` & `MessageMetaData`

Note: the list of File Attachments can be passed to the helper method ReassembleChunkedFile as below which will return a File Attachment.

```c#
    var assembledFile = await FileHelpers.ReassembleChunkedFile(getMessageResponse.Response.FileAttachments);
```

#### Get Head Message By Id

This method will retrieve a message metadata based on the message_id obtained from the 'GetMessagesAsync' method.

```c#
    var result = await _meshInboxService.GetHeadMessageByIdAsync(mailboxId, messageId);
```

this response will return an object with a `MessageMetaData` property.

#### Acknowledge Message By Id

This method will acknowledge the successful download of a message.

```c#
    var result = await _meshInboxService.AcknowledgeMessageByIdAsync(mailboxId, messageId);
```

this will return the Id of the message acknowledge.

### Mesh Outbox Service

To use this inject `IMeshOutboxService` class as shown in [Usage](#usage).
This class contains methods used for sending messages to MESH.

All of the sending methods will expect the below list of parameters

| **Parameter Name** | **Data Type**  | **Required** | **Description**                                                                           |
|--------------------|----------------|--------------|-------------------------------------------------------------------------------------------|
| fromMailboxId      | string         | Y            | Sender mailbox Id                                                                         |
| toMailboxId        | string         | Y            | Recipient mailbox ID                                                                      |
| workFlowId         | string         | Y            | Identifies the type of message being sent e.g. Pathology, GP Capitation.                  |
| file               | FileAttachment | Y            | contains the details of the file to be sent                                               |
| localId            | string         | N            | local identifier, your reference                                                          |
| subject            | string         | N            | additional message subject                                                                |
| includeChecksum    | bool           | N            | By default this is false, if true a header will be added with an MD5 Checksum of the file |

### Testing

There are `make` tasks for you to configure to run your tests.  Run `make test` to see how they work.  You should be able to use the same entry points for local development as in your CI pipeline.

## Licence

> The [LICENCE.md](./LICENCE.md) file will need to be updated with the correct year and owner

Unless stated otherwise, the codebase is released under the MIT License. This covers both the codebase and any sample code in the documentation.

Any HTML or Markdown documentation is [© Crown Copyright](https://www.nationalarchives.gov.uk/information-management/re-using-public-sector-information/uk-government-licensing-framework/crown-copyright/) and available under the terms of the [Open Government Licence v3.0](https://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/).
