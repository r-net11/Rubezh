using System;
using System.IO;
using RubezhAPI.GK;
using RubezhAPI.SKD;
using Infrastructure.Common.TreeList;
using Softing.Opc.Ua.Toolkit;

namespace AutomationModule.ViewModels
{
	public class TagViewModel : TreeNodeViewModel<TagViewModel>
	{
        public ReferenceDescription Tag { get; private set; }
	    public Guid UID { get; set; }
	    public string Address { get; set; }
        public string Name { get; set; }
	    public bool IsTagUsed { get; set; }
	    public string Path { get; set; }

	    public TagViewModel(ReferenceDescription tag)
		{
			Tag = tag;
	        UID = Guid.NewGuid();
	        Address = tag != null ? tag.NodeId.ToString() : "Root";
	        Name = tag != null ? tag.DisplayName.ToString() : "Name";
	        Path = "";
		}


        public bool CanUseCheck
        {
            get { return ChildrenCount == 0; }
            
        }

	}
}