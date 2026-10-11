using System;
using System.IO;

namespace FleetMaintenance.Core;

public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    public FileAuditLog(string path)
    {
        _writer = new StreamWriter(path, append: true);
    }

    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer.WriteLine($"{DateTimeOffset.Now:O};{operation};{documentId}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
