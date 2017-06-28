using ContactList.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Net.Http;
using System;

namespace ZrappoDummyAPI.Controllers
{
    public class VouchersController : ApiController
    {
        private const string FILENAME = "contacts.json";
        private GenericStorage _storage;

        public VouchersController()
        {
            _storage = new GenericStorage();
        }

        private async Task<IEnumerable<Voucher>> GetVouchers()
        {
            var vouchers = await _storage.GetVouchers(FILENAME);

            if (vouchers == null)
            {
                await _storage.SaveVouchers(new Voucher[]
                {
                    new Voucher()
                    {
                        Id = Guid.NewGuid(),
                        VoucherDate = DateTime.Now,
                        VoucherText = $"Dagskassa {DateTime.Now}",
                        Rows = new VoucherRow[]
                        {
                            new VoucherRow
                            {
                                AccountNumber = 1930,
                                DebitAmount = 100,
                                CreditAmount = 0
                            },
                            new VoucherRow
                            {
                                AccountNumber = 3051,
                                DebitAmount = 0,
                                CreditAmount = 80
                            },
                            new VoucherRow
                            {
                                AccountNumber = 2611,
                                DebitAmount = 0,
                                CreditAmount = 20
                            }
                        }
                    }
                }
                , FILENAME);
            }

            return vouchers;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(IEnumerable<Voucher>))]
        [Route("~/vouchers")]
        public async Task<IEnumerable<Voucher>> Get()
        {
            return await GetVouchers();
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "OK",
            Type = typeof(IEnumerable<Voucher>))]
        [SwaggerResponse(HttpStatusCode.NotFound,
            Description = "Voucher not found",
            Type = typeof(IEnumerable<Voucher>))]
        [SwaggerOperation("GetVouchersById")]
        [Route("~/vouchers/{id}")]
        public async Task<Voucher> Get([FromUri] Guid id)
        {
            var vouchers = await GetVouchers();
            return vouchers.FirstOrDefault(x => x.Id == id);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created,
            Description = "Created",
            Type = typeof(Voucher))]
        [Route("~/vouchers")]
        public async Task<Voucher> Post([FromBody] Voucher voucher)
        {
            var vouchers = await GetVouchers();
            var voucherList = vouchers.ToList();
            voucherList.Add(voucher);
            await _storage.SaveVouchers(voucherList, FILENAME);
            return voucher;
        }

        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK,
           Description = "OK",
           Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.NotFound,
           Description = "Voucher not found",
           Type = typeof(bool))]
        [Route("~/vouchers/{id}")]
        public async Task<HttpResponseMessage> DeleteVouchers([FromUri] Guid id)
        {
            var vouchers = await GetVouchers();
            var voucherList = vouchers.ToList();

            if (!voucherList.Any(x => x.Id == id))
            {
                return Request.CreateResponse<bool>(HttpStatusCode.NotFound, false);
            }
            else
            {
                voucherList.RemoveAll(x => x.Id == id);
                await _storage.SaveVouchers(voucherList, FILENAME);
                return Request.CreateResponse<bool>(HttpStatusCode.OK, true);
            }
        }
    }
}