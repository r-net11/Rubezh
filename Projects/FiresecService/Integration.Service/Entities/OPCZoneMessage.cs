namespace Integration.Service.Entities
{
	internal sealed class OPCZoneMessage
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public string Description { get; set; }
		public string Autoset { get; set; }
		public string Delay { get; set; }
		public string GuardZoneType { get; set; }
		public string IsSkippedTypeEnabled { get; set; }
		public string ZoneType { get; set; }
	}
}
