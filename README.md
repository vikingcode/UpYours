# UpYours

A YouTube uploader for Windows that auto-imports metadata.

<VideoName>.mp4 searches for <VideoName>.txt for the metadata, and <VideoName>.jpg for the thumbnail.

Provide your own client_id.json by creating a developer account with google, registering an app, etc.

###The text metadata format

```
Title
Category
status
rest of the file makes up the description
```

The category matches (currently case sensitive) the text from YouTube's categories, not the IDs.
Status can be `private`, `public`, or `unlisted`

This format is subject to change while I add features.

See Examples\DrawerOrganiser.txt