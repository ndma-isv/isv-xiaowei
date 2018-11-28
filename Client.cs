using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISV.Xiaowei
{
    /*
     此类库为微信支付服务商小微接口进件演示代码，有问题可联系
     开发商: NDMA&ISV
     微信: ms-edge
     Email: isv@uxos.net         
    */

    public class Client
    {
        private Dictionary<string, string> _dict = new Dictionary<string, string>();

        public Client(string cert_sn, string mch_id)
        {
            // 基本参数和服务商信息传递
            this["version"] = "2.0";
            this["sign_type"] = "HMAC-SHA256";
            this["cert_sn"] = cert_sn;
            this["mch_id"] = mch_id;
            this["nonce_str"] = Guid.NewGuid().ToString("n");



            // 由图片上传接口预先上传图片生成好的 media_id 
            this["id_card_copy"] = this.GetMediaId("test");
            this["id_card_national"] = this.GetMediaId("test");
            this["store_entrance_pic"] = this.GetMediaId("test");
            this["indoor_pic"] = this.GetMediaId("test");


            // 以下参数根据商户实际信息更改，参考https://pay.weixin.qq.com/wiki/doc/api/xiaowei.php?chapter=19_2
            this["business_code"] = "123456"; // 业务申请编号，本地数据ID
            this["id_card_valid_time"] = "[\"2005-11-03\",\"长期\"]"; // 身份证有效期
            this["business"] = "551"; // 经营类目ID 
            this["account_bank"] = "农业银行";
            this["bank_address_code"] = "110000";
            this["store_name"] = "门店名称";
            this["store_address_code"] = "110000";
            this["store_street"] = "具体区县及街道门牌号或大厦楼层";
            this["merchant_shortname"] = "商户名称";
            this["service_phone"] = "13800138000";
            this["rate"] = "0.38%"; // 结算费率
        }

        public string this[string key]
        {
            get => this._dict[key];
            set => this._dict[key] = value;
        }

        public string GetMediaId(string name)
        {
            // 此为示例ID，应先使用图片上传接口获得
            return "fUqH4b7wMfZmE2gPuhrbRM4W3L0pPbaik1Z3-ud82sDS3pceTqj-F_QPr60rtye1j7Zm5kMMNpsD0W-0Apr55e7dzyBRFDjdZk6rqfaZ12I";
        }

        public string Submit(string key, X509Certificate2 cert)
        {
            this["sign"] = Utils.Sign(this._dict, key);
            var xml = new XElement("xml");
            foreach (var kv in this._dict)
            {
                xml.Add(new XElement(kv.Key, kv.Value));
            }

            try
            {
                return this.Post(xml.ToString(), cert);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string Post(string text, X509Certificate2 cert)
        {
            var addr = "https://api.mch.weixin.qq.com/applyment/micro/submit";

            var request = WebRequest.Create(addr) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = WebRequestMethods.Http.Post;
            request.UserAgent = "Mozilla/5.0";
            request.ContentType = "text/xml";
            request.ClientCertificates.Add(cert);

            var buff = Encoding.UTF8.GetBytes(text);
            using (var stream = request.GetRequestStream())
            {
                stream.Write(buff, 0, buff.Length);
            }

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}