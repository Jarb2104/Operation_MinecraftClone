namespace WorldBuilding.CustomErrors
{
    public class BlockOutOfRangeException : Exception
    {
        public BlockOutOfRangeException(string message) : base(message)
        {
        }
    }

    public class ChunkOutOfRangeException : Exception
    {
        public ChunkOutOfRangeException(string message) : base(message)
        {
        }
    }
}