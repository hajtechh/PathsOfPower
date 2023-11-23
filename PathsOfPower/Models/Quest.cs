using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathsOfPower.Models;

public class Quest
{
    public string Index { get; set; }
    public string Description { get; set; }
    public IEnumerable<Option>? Options { get; set; }
}
