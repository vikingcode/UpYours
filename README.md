# UpYours

A YouTube uploader for Windows that auto-imports metadata.

<VideoName>.mp4 searches for <VideoName>.txt for the metadata, and <VideoName>.jpg for the thumbnail.

###The text metadata format

```
Title
Category
status
rest of the file makes up the description
```

The category matches (currently case sensitive) the text from YouTube's categories, not the IDs.
Status can be `private`, `public`, or `unlisted`
