<?php
static class SendTieba // 隶属于Core类
{
    public function StdPost($path, $post = '', $bduss = '') // HttpPost的贴吧安全封装版
    {
        $posting = 'BDUSS=' . urlencode($bduss) . '&_client_id=wappc_1397878440657_358&&_client_type=2' . '&_client_version=6.0.0&_phone_imei=000000000000000';
        if (strlen($post) != 0)
            $posting .= '&' . $post;
        $sign = explode('&', $posting);
        usort($sign, array(
            $this,
            'Opentieba_Comparer'
        )); // 相当于访问本类的Opentieba_Comparer方法（对照C#）
        $sign = implode('', $sign) . 'tiebaclient!!!';
        $sign = urldecode($sign);
        $sign = md5($sign);
        $posting .= '&sign=' . strtoupper($sign);
        return $this->HttpPost("http://c.tieba.baidu.com" . $path, $posting);
    }
    public function HttpPost($url, $post = '', $cookie = '') // Curl的封装版
    {
        $header[] = 'Accept: */*';
        $header[] = 'User-Agent: Mozilla/4.0 (Windows NT 5.1)';
        $header[] = 'Connection: Keep-Alive';
        $header[] = 'Cookie: ' . $cookie;
        $curl     = curl_init();
        curl_setopt($curl, CURLOPT_URL, $url);
        curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
        curl_setopt($curl, CURLOPT_HTTPHEADER, $header);
        curl_setopt($curl, CURLOPT_POST, 1);
        curl_setopt($curl, CURLOPT_POSTFIELDS, $post);
        $ret = curl_exec($curl);
        curl_close($curl);
        return $ret;
    }
    private function getBytes($string) // 排序器函数调用
    {
        $bytes = array();
        for ($i = 0; $i < strlen($string); $i++) {
            $bytes[] = ord($string[$i]);
        }
        return $bytes;
    }
    private function Opentieba_Comparer($a, $b) // 排序器函数
    {
        $a_ = $this->getBytes($a);
        $b_ = $this->getBytes($b);
        if (count($a_) < count($b_))
            array_pad($a_, count($b_), 0);
        if (count($b_) < count($a_))
            array_pad($b_, count($a_), 0);
        for ($i = 0; $i < count($a_); ++$i) {
            if ($a_[$i] < $b_[$i])
                return -1;
            if ($a_[$i] > $b_[$i])
                return 1;
        }
        return 0;
    }
}
?>