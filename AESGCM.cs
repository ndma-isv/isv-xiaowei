using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Text;

namespace ISV.Xiaowei
{
     /*
     此类库为微信支付服务商小微接口进件演示代码，有问题可联系
     开发商: NDMA&ISV
     微信: ms-edge
     Email: isv@uxos.net         
    */

    public class AESGCM
    {
        /// <summary>
        /// 敏感信息加密需要从https://api.mch.weixin.qq.com/risk/getcertficates此接口下载加密证书进行下一步加密，
        /// 该接口下载到的是密文，使用此AESGCM.Decrypt()方法解密得到证书明文
        /// </summary>
        /// <param name="ciphertext">接口下载得到的JSON里的ciphertext字段</param>
        /// <param name="key">微信支付商户后台设置的V3密钥</param>
        /// <param name="ivs">接口下载得到的JSON里的nonce字段</param>
        /// <param name="associatedText">默认为certificate，不需更改</param>
        /// <returns> 返回公钥明文，-----BEGIN CERTIFICATE----- </returns>
        public static string Decrypt(string ciphertext, string key, string ivs, string associatedText = "certificate")
        {
            var buff = Convert.FromBase64String(ciphertext);
            var secret = Encoding.UTF8.GetBytes(key);
            var nonce = Encoding.UTF8.GetBytes(ivs);
            var associatedData = Encoding.UTF8.GetBytes(associatedText);

            // 算法 AEAD_AES_256_GCM，C# 环境使用 BouncyCastle.Crypto.dll 类库实现
            var cipher = new GcmBlockCipher(new AesFastEngine());
            var aead = new AeadParameters(new KeyParameter(secret), 128, nonce, associatedData);
            cipher.Init(false, aead);

            var data = new byte[cipher.GetOutputSize(buff.Length)];
            var num = cipher.ProcessBytes(buff, 0, buff.Length, data, 0);
            try
            {
                cipher.DoFinal(data, num);
            }
            catch (Exception)
            {
            }

            return Encoding.UTF8.GetString(data);
        }
    }
}
