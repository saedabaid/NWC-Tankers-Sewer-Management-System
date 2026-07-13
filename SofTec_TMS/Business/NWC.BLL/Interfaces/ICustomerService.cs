using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;

namespace NWC.BLL.Interfaces
{
    public interface ICustomerService
    {
        DescriptiveResponse<CustomerDTO> CreateCustomer(CustomerDTO dto);
        DescriptiveResponse<CustomerLocationDTO> CreateCustomerLocation(CustomerLocationDTO dto);
        DescriptiveResponse<CustomerLocationDTO> GetCustomerLocByIntegrationID(string integrationID);
        DescriptiveResponse<CustomerDTO> CreateCustomerAndLocations(CustomerDTO dto);
        DescriptiveResponse<bool> EditCustomerAndLocations(CustomerDTO dto);
        DescriptiveResponse<bool> DeleteCustomer(long customerId);

        DescriptiveResponse<CustomerAccountDTO> CreateCustomerAccount(CustomerAccountDTO dto);
        DescriptiveResponse<bool> AddSoqyaCustomerBalance(SoqyaCustomerBalanceDTO dto);
        DescriptiveResponse<SoqyaCustomerBalanceDTO> CreateCustomerBalance(SoqyaCustomerBalanceDTO dto);
        DescriptiveResponse<SearchResult<CustomerDTO>> SearchCustomerList(CustomerSC searchCriteria);
        DescriptiveResponse<SearchResult<CustomerAccountDTO>> SearchCustomerAccountList(CustomerAccountSC searchCriteria);


        string InsertCustomerAccounts();
        string UpdateWorkOrders();
    }
}
