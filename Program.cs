using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ISV.Xiaowei
{
    class Program
    {
        /*
         此类库为微信支付服务商小微接口进件演示代码，有问题可联系
         开发商: NDMA&ISV
         微信: ms-edge
         Email: isv@uxos.net         
        */

        static void Main(string[] args)
        {
            var cert_sn = ""; // 证书系列号
            var mch_id = "1000077001"; // 服务商户号
            var secret = ""; // 服务商密钥
            var path = @"cert.p12"; // 证书路径
            var cert = new X509Certificate2(path, mch_id); // 从本地加载微信支付商户后台下载的 p12 证书文件
            var publicKey = File.ReadAllBytes(@"public.pem"); // 从平台接口下载到的公钥

            // 初始化请求类，并完善参数，加密敏感信息
            var client = new Client(cert_sn, mch_id);
            client["contact"] = Utils.RSAEncrypt("姓名", publicKey);
            client["id_card_name"] = Utils.RSAEncrypt("姓名", publicKey);
            client["account_name"] = Utils.RSAEncrypt("姓名", publicKey);
            client["contact_phone"] = Utils.RSAEncrypt("13800138000", publicKey);
            client["id_card_number"] = Utils.RSAEncrypt("460106198404060137", publicKey);
            client["account_number"] = Utils.RSAEncrypt("460106198404060137", publicKey);


            // 发起请求，返回完整XML即通讯成功，XML提示签名失败则secret错误，XML提示解密敏感信息解密失败则加密公钥用错 
            var result = client.Submit(secret, cert);
            Console.WriteLine(result);
            Console.ReadLine();


            /* 请求成功时，返回如下XML
             * 
                <xml><return_code><![CDATA[SUCCESS]]></return_code>
                <return_msg><![CDATA[OK]]></return_msg>
                <nonce_str><![CDATA[PRmBNusxC7OzkLQA]]></nonce_str>
                <sign><![CDATA[5F215605EBF93BFCD3573B78594B43349DED665630737E1ED65AA215EA5B360A]]></sign>
                <result_code><![CDATA[SUCCESS]]></result_code>
                <applyment_id><![CDATA[2000000000428996]]></applyment_id>
                </xml>
 
             * */
        }
    }
}
