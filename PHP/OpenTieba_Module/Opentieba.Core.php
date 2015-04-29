<?php
class Core // ���ģ��й�����ͨ�ż�BDUSS����
{
    public function Login($username, $password, $vcodemd5 = '', $vcode = '', &$ret = null) // ��¼������Username��Password��VerifyCodeMD5����֤���ʶ����VerifyCode������Ĭ��Ϊ�գ�������Boolean����ʹ���������+1
    {
        /*
        �ص�Ϊ�����ࣺ
        ���������ΪTrue��
        �ص�����BDUSS��UserID��
        ���������ΪFalse��
        �ص�������ErrorCode��ErrorMessage��NeedVerifyCode��VerifyCodeMD5��VerifyCodePictureURL��
        ��VerifyCodeMD5��VerifyCodePictureURLȡ����NeedVerifyCode��
        */
        $posting = 'un=' . urlencode($username) . '&passwd' + urlencode(base64_encode($password)) . '&vcode_md5=' . $vcodemd5 . '&vcode' . urlencode($vcode)
        $output = $this->StdPost('/c/s/login', $posting);
        $output = json_decode($output)
        if ($output == false) return false;
        if ($output->error_code != 0)
        {
            $ret = (object)array (
                                    'errcode' => $output->error_code,
                                    'uid' => mb_convert_encoding($output->error_msg, 'gb2312', 'utf-8'),
                                    'needvcode' => false,
                                    'vcodemd5' => '',
                                    'vcodeurl' => ''
                                 )
            if (isset($output->anti->need_vcode) and $output->anti->need_vcode == 1)
            {
                $ret->needvcode = true;
                $ret->vcodemd5 = $output->anti->vcode_md5;
                $ret->vcodeurl = $output->anti->vcode_pic_url;
            }
            return false;
        }
        else
        {
            $bduss_ = $output->user->BDUSS;
            $uid_ = $output->user->id;
            $ret = (object)array (
                                    'bduss' => $bduss_ ,
                                    'uid' => $uid_
                                 )
            return true;
        }
    }
    public function Logout() { return false; } // �ǳ�����ʵ�֡�
    public function StdPost($path, $post = '', $bduss = '') // HttpPost�����ɰ�ȫ��װ��
    {
        $posting = 'BDUSS=' . urlencode($bduss) . '&_client_id=wappc_1397878440657_358&&_client_type=2' . '&_client_version=6.0.0&_phone_imei=000000000000000';
        if (strlen($post) != 0)
            $posting .= '&' . $post;
        $sign = explode('&', $posting);
        usort($sign, array(
            $this,
            'Opentieba_Comparer'
        )); // �൱�ڷ��ʱ����Opentieba_Comparer����������C#��
        $sign = implode('', $sign) . 'tiebaclient!!!';
        $sign = urldecode($sign);
        $sign = md5($sign);
        $posting .= '&sign=' . strtoupper($sign);
        return $this->HttpPost("http://c.tieba.baidu.com" . $path, $posting);
    }
    public function HttpPost($url, $post = '', $cookie = '') // Curl�ķ�װ��
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
    private function getBytes($string) // ��������������
    {
        $bytes = array();
        for ($i = 0; $i < strlen($string); $i++) {
            $bytes[] = ord($string[$i]);
        }
        return $bytes;
    }
    private function Opentieba_Comparer($a, $b) // ����������
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