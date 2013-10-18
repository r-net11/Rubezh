
namespace Infrastructure.Common
{
	public class PatchIndex
    {
        public string Type { get; private set; }
        public string Id { get; private set; }

        public PatchIndex(string type, string id)
        {
            Type = type;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            var patchIndex = (PatchIndex)obj;
            return Type == patchIndex.Type && Id == patchIndex.Id;
        }
    }
}