namespace Genocs.QRCodeLibrary.Decoder;

#if DEBUG
/////////////////////////////////////////////////////////////////////
// Trace Class
/////////////////////////////////////////////////////////////////////

static public class QRCodeTrace
{
    private static string TraceFileName;        // trace file name
    private static int MaxAllowedFileSize = 1024 * 1024;
    private static bool isEmpty = true;

    /////////////////////////////////////////////////////////////////////
    // Open trace file
    /////////////////////////////////////////////////////////////////////

    public static void Open(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        isEmpty = false;

        // save full file name
        TraceFileName = Path.GetFullPath(fileName);
        Write("----");
        return;
    }

    /////////////////////////////////////////////////////////////////////
    // write to trace file
    /////////////////////////////////////////////////////////////////////

    public static void Format(string message, params object[] args)
    {
        if (isEmpty) return;
        if (args.Length == 0) Write(message);
        else Write(string.Format(message, args));
        return;
    }

    /////////////////////////////////////////////////////////////////////
    // write to trace file
    /////////////////////////////////////////////////////////////////////

    public static void Write(string Message)
    {
        if (isEmpty) return;

        // test file length
        TestSize();

        // open existing or create new trace file
        StreamWriter TraceFile = new StreamWriter(TraceFileName, true);

        // write date and time
        TraceFile.Write(string.Format("{0:yyyy}/{0:MM}/{0:dd} {0:HH}:{0:mm}:{0:ss} ", DateTime.Now));

        // write message
        TraceFile.WriteLine(Message);

        // close the file
        TraceFile.Close();

        // exit
        return;
    }


    /////////////////////////////////////////////////////////////////////
    // Test file size
    // If file is too big, remove first quarter of the file
    /////////////////////////////////////////////////////////////////////

    private static void TestSize()
    {
        if (isEmpty) return;

        // get trace file info
        FileInfo TraceFileInfo = new FileInfo(TraceFileName);

        // if file does not exist or file length less than max allowed file size do nothing
        if (TraceFileInfo.Exists == false || TraceFileInfo.Length <= MaxAllowedFileSize) return;

        // create file info class
        FileStream TraceFile = new FileStream(TraceFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        // seek to 25% length
        TraceFile.Seek(TraceFile.Length / 4, SeekOrigin.Begin);

        // new file length
        int NewFileLength = (int)(TraceFile.Length - TraceFile.Position);

        // new file buffer
        byte[] Buffer = new byte[NewFileLength];

        // read file to the end
        TraceFile.Read(Buffer, 0, NewFileLength);

        // search for first end of line
        int StartPtr = 0;
        while (StartPtr < 1024 && Buffer[StartPtr++] != '\n') ;
        if (StartPtr == 1024) StartPtr = 0;

        // seek to start of file
        TraceFile.Seek(0, SeekOrigin.Begin);

        // write 75% top part of file over the start of the file
        TraceFile.Write(Buffer, StartPtr, NewFileLength - StartPtr);

        // truncate the file
        TraceFile.SetLength(TraceFile.Position);

        // close the file
        TraceFile.Close();

        // exit
        return;
    }
}
#endif
