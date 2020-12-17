/* using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace APICallHandler
{
    public interface IActionable
    {
        IEnumerable<IActionable> Get();
        IEnumerable<IActionable> Get(IEnumerable<Tuple<string,string>> Criteria);
        IActionable Get(long Id);

        IEnumerable<long> Delete(IEnumerable<long> IdList);        
        long Delete(long Id);        

        IEnumerable<long> Create(IEnumerable<IActionable> ItemList);
        IEnumerable<long> Create(IEnumerable<IEnumerable<string>> ItemListValues);
        IEnumerable<long> Create(JsonDocument JsonItemList);
        long Create(IActionable Item);
        long Create(IEnumerable<string> ItemValues);

        long Update(IActionable Item);
        long Update(IEnumerable<string> ItemValues);
        IEnumerable<long> Update(IEnumerable<IActionable> ItemList);
        IEnumerable<long> Update(IEnumerable<IEnumerable<string>> ItemListValues);
        IEnumerable<long> Update(JsonDocument JsonItemList);



    }
}
*/