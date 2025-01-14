# Azure AI Content Safety API
Container Function app integrated with Azure AI Content Safety to moderate text or image contents.

## About

Containerized azure function app to post text or images (jpg, jpeg, bmp, png and tiff) on Azure AI Content Safety server which reads the contents and returns the content safety response against Hate, SelfHarm, Sexual or Violence Severity level between 0 to 6, higher the value mean contern is unsafe to publish.

## Setup Content Safety Service in Azrue.

Setup [What is Azure AI Content Safety? - Azure AI services | Microsoft Learn](https://learn.microsoft.com/en-gb/azure/ai-services/content-safety/overview) manually via the Azure Portal, and polpulated the Local setting files with "Key" and "endpoint", for the PoC you can use Free tier.

Function app local setting file.

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ContentSafetyApiKey": "{key from portal}",
    "ContentSafetyApiEndpoint": "https://{ your content safety from portal}.cognitiveservices.azure.com/"
  }
}

```

Also, please [Install docker desktop](https://www.docker.com/get-started/) if not exist on your machine to the function app inside a docker container.


## Setup Azure AI Content Safety infrastructure via Terraform

To setup [Cognitive Services Account](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs/resources/cognitive_account) via Terraform and set the Kind attribute to 'ContentSafety' and sku_name = F0 or F1 to use free tier, otherwise S0 to keep the cost low.

```

resource "azurerm_resource_group" "ai-example" {
  name     = "ai-example-resources"
  location = "West Europe"
}

resource "azurerm_cognitive_account" "ai-example" {
  name                = "ai-example-account"
  location            = azurerm_resource_group.ai-example.location
  resource_group_name = azurerm_resource_group.ai-example.name
  kind                = "ContentSafety"

  sku_name = "F0"

  tags = {
    Acceptance = "Test"
  }
}

```

## Additional documentation
To know more in details please find below additional azure AI content safety contents.

1. [Azure AI Content Safety Studio](https://learn.microsoft.com/en-gb/azure/ai-services/content-safety/studio-quickstart)
2. [QuickStart: Analyze text content](https://learn.microsoft.com/en-gb/azure/ai-services/content-safety/quickstart-text?tabs=visual-studio%2Cwindows&pivots=programming-language-csharp)
3. [QuickStart: Analyze content content](https://learn.microsoft.com/en-gb/azure/ai-services/content-safety/quickstart-image?tabs=visual-studio%2Cwindows&pivots=programming-language-csharp)
