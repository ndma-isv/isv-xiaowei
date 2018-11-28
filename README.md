# isv-xiaowei
微信支付服务商小微进件接口

2018版新小微进件接口逻辑较复杂，步骤如下：
            1、升级CA权威新证书并下载
            2、设置V3密钥
            3、使用https://api.mch.weixin.qq.com/risk/getcertficates  接口获取敏感信息加密公钥密文
            4、使用此类库 AESGCM.Decrypt() 方法AEAD_AES_256_GCM算法解密获取实际的敏感信息加密公钥
            5、调用主代码传递相关服务商信息发起进件请求

注：进件的身份证、门店照片应事先用图片上传接口上传并获得media_id，为避免编写复杂的C#代码，可使用System.Diagnostics.Process类调用curl上传图片，并读取media_id，接口文档 https://pay.weixin.qq.com/wiki/doc/api/download/img_upload.pdf

            var cert_sn = ""; // 加密证书系列号
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
