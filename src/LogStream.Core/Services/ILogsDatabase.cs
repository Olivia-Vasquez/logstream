using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogStream.Core.Services
{
    public interface ILogsDatabase
    {
        Task<int> InsertItemAsync(Models.Item item);
        Task<int> InsertItemDetailAsync(Models.ItemDetail detail);
        Task<int> InsertItemDetailsAsync(IEnumerable<Models.ItemDetail> details);
        Task<List<Models.Item>> GetItemsAsync();
        Task<List<Models.ItemDetail>> GetItemDetailsAsync(int itemId);
        Task<int> DeleteItemAsync(Models.Item item);
        Task<int> UpdateItemAsync(Models.Item item);
    }
}
