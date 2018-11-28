using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ISV.Xiaowei
{
    /*
     此类库为微信支付服务商小微接口进件演示代码，有问题可联系
     开发商: NDMA&ISV
     微信: ms-edge
     Email: isv@uxos.net         
    */

    public static class Utils
    {
        /// <summary>
        /// 微信支付新接口均使用了 HMAC-SHA256 签名算法 
        /// </summary>
        /// <param name="dict">请求的参数对</param>
        /// <param name="key">微信支付商户后台设置的密钥</param>
        /// <returns>HMACSHA256 计算得到的签名值</returns>
        public static string Sign(Dictionary<string, string> dict, string key)
        {
            var q = from kv in dict
                    where string.IsNullOrEmpty(kv.Value) == false
                    orderby kv.Key
                    select string.Join("=", kv.Key, kv.Value);

            var str = string.Join("&", q);
            str = $"{str}&key={key}";

            using (var sha = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                sha.Initialize();

                var hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", null);

                return hash;
            }
        }

        /// <summary>
        /// 最终提交请求时，需对敏感信息加密，如身份证、银行卡号。
        /// 加密算法是RSA，使用从接口下载到的公钥进行加密，非后台下载到的私钥。
        /// 
        /// </summary>
        /// <param name="text">要加密的明文</param>
        /// <param name="publicKey"> -----BEGIN CERTIFICATE----- 开头的string，转为bytes </param>
        /// <returns></returns>
        public static string RSAEncrypt(string text, byte[] publicKey)
        {
            using (var x509 = new X509Certificate2(publicKey))
            {
                using (var rsa = (RSACryptoServiceProvider)x509.PublicKey.Key)
                {
                    var buff = rsa.Encrypt(Encoding.UTF8.GetBytes(text), false);

                    return Convert.ToBase64String(buff);
                }
            }
        }
    }
}