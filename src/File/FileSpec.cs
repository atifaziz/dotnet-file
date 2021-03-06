﻿using System;

namespace Microsoft.DotNet
{
    public class FileSpec
    {
        public FileSpec(Uri uri)
            : this(System.IO.Path.GetFileName(uri.LocalPath), uri)
        {
        }

        public FileSpec(string path, Uri? uri = null, string? etag = null)
        {
            Path = path;
            Uri = uri;
            ETag = etag;
        }

        public string Path { get; }

        public Uri? Uri { get; }

        public string? ETag { get; }
    }
}
