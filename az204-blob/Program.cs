﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


Console.WriteLine("Azure Blob Storage exercise\n");

// Run the examples asynchronously, wait for the results before proceeding
ProcessAsync().GetAwaiter().GetResult();

Console.WriteLine("Press enter to exit the sample application.");
Console.ReadLine();

static async Task ProcessAsync()
{

    // Copy the connection string from the portal in the variable below.
    string storageConnectionString = "superSecret_storageconnectionString";

    // Create a client that can authenticate with a connection string
    BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

    //Create string vatiable for unique name for the container
    string containerName = "trinisblob" + Guid.NewGuid().ToString();

    // Class to create a container and return a container
    BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
    Console.WriteLine("A container named '" + containerName + "' has been created. " +
        "\nTake a minute and verify in the portal." +
        "\nNext a file will be created and uploaded to the container.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // Create a local file in the ./data/ directory for uploading and downloading
    string localPath = "./data/";
    string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
    string localFilePath = Path.Combine(localPath, fileName);

    // Write text to the file
    await File.WriteAllTextAsync(localFilePath, "Hello, World!");

    // Get a reference to the blob
    BlobClient blobClient = containerClient.GetBlobClient(fileName);

    Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

    // Open the file and upload its data
    using (FileStream uploadFileStream = File.OpenWrite(localFilePath))
    {
        await blobClient.UploadAsync(uploadFileStream);
        uploadFileStream.Close();
    }

    Console.WriteLine("\nThe file was uploaded. We'll verify by listing" +
            " the blobs next.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // List blobs in the container
    Console.WriteLine("Listing blobs...");
    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        Console.WriteLine("\t" + blobItem.Name);
    }

    Console.WriteLine("\nYou can also verify by looking inside the " +
            "container in the portal." +
            "\nNext the blob will be downloaded with an altered file name.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // Download the blob to a local file
    // Append the string "DOWNLOADED" before the .txt extension 
    string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

    Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

    // Download the blob's contents and save it to a file
    BlobDownloadInfo download = await blobClient.DownloadAsync();

    using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
    {
        await download.Content.CopyToAsync(downloadFileStream);
    }
    Console.WriteLine("\nLocate the local file in the data directory created earlier to verify it was downloaded.");
    Console.WriteLine("The next step is to delete the container and local files.");
    Console.WriteLine("Press 'Enter' to continue.");
    Console.ReadLine();

    // Delete the container and clean up local files created
    Console.WriteLine("\n\nDeleting blob container...");
    await containerClient.DeleteAsync();

    Console.WriteLine("Deleting the local source and downloaded files...");
    File.Delete(localFilePath);
    File.Delete(downloadFilePath);

    Console.WriteLine("Finished cleaning up.");

}