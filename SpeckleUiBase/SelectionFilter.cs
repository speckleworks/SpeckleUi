using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleUiBase
{
  public interface ISelectionFilter
  {
    string Name { get; set; }
    string Icon { get; set; }
    string Type { get; } 
  }

  public class ElementsSelectionFilter : ISelectionFilter
  {
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Type { get { return typeof(ElementsSelectionFilter).ToString(); }  }

    public int Count { get; set; }
  }

  public class ListSelectionFilter : ISelectionFilter
  {
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Type { get { return typeof(ListSelectionFilter).ToString(); } }

    public List<string> Values { get; set; }
    public List<string> Selection = new List<string>();
  }

  public class CustomSelectionFilter : ISelectionFilter
  {
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Type { get { return typeof(CustomSelectionFilter).ToString(); } }

    public List<string> Values { get; set; }
    public List<string> Selection = new List<string>();
  }






}
