using System;
using System.Collections.Generic;
using System.Text;

namespace kaiten.Models
{
  class JiraConfiguration
  {
    public String ProjectName { get; set; }
    public String Password { get; set; }
    public String Login { get; set; }
    public String Url { get; set; }
    public String ParentId { get; set; }
    public String UserName { get; set; }
    public String[] Components { get; set; }
    public String[] Labels { get; set; }
  }
}
