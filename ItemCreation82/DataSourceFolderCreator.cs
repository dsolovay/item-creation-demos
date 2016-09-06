using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace ItemCreation82
{
  public class DataSourceFolderCreator
  {
    public void Process(GetRenderingDatasourceArgs args)
    {
      var renderingItem = args.RenderingItem;

      if (!renderingItem.GetChildren().Any(item => item.Name == "Items" && item.TemplateID == TemplateIDs.Folder))
        renderingItem.Add("Items", new TemplateID(TemplateIDs.Folder));
    }
  }
}