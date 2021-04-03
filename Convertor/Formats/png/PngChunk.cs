namespace Convertor.Formats.png
{
    public class PngChunk
    {
        private PngChunkType pngChunkType;
        private byte[] chunkBody;

        public PngChunkType PngChunkType
        {
            get => pngChunkType;
            set => pngChunkType = value;
        }

        public byte[] ChunkBody
        {
            get => chunkBody;
            set => chunkBody = value;
        }
        
        public override string ToString()
        {
            return "Chunk: " + pngChunkType + ", B.L.:" + chunkBody.Length;
        }
    }
}