import requests
import urllib3
from urllib.parse import quote
s = open("setting.txt", "r")
settingArr=s.read().split("|")
f = open("text.txt", "r", encoding='utf-8') 
url = "https://zalo.ai/api/demo/v1/tts/synthesize"
text =quote(str(f.read()))

# text.encode('utf-8')  # Totally fine.
payload = "input="+text+"&speaker_id="+settingArr[0]+"&speed="+settingArr[1]+"&dict_id=0"
headers = {
    "content-type": "application/x-www-form-urlencoded; charset=utf-8",
    "origin": "https://zalo.ai",
    "referer": "https://zalo.ai/experiments/text-to-audio-converter",

    "cookie": "zpsid=eMKnVbo-PZEvNHqtDTKIOgHQ7p4nrWzalI47O4wZJssuT3bRV_irVuyWFcWShorgrNnyH1sN7H_cHL08DySx4jayN3Kgv2SblZf95sovCHgQRaSg; zai_did=8k9uAj3FNiTevcSSryzXoYYo64d0o6V3AB4PHJ8q; zpsidleg=eMKnVbo-PZEvNHqtDTKIOgHQ7p4nrWzalI47O4wZJssuT3bRV_irVuyWFcWShorgrNnyH1sN7H_cHL08DySx4jayN3Kgv2SblZf95sovCHgQRaSg; zai_sid=lf2zTzCfGqIZbxznrofUGhhifo2eNnvBlxcP6va7P5c8xPue-bDyJDAnt0JxQqmvuOZmID4xQZJUyVnrp1Xs0xdtwLUAHM0ydQFdQl1IIGRigkzd; __zi=3000.SSZzejyD0jydXQcYsa00d3xBfxgP71AM8Tdbg8yB7SWftQxdY0aRp2gIh-QFHXF2BvMWxp0mDW.1; fpsend=149569; _zlang=vn"
}

response = requests.request("POST", url, data=payload.encode('utf-8'), headers=headers)
f = open("output.txt", "w")
f.write(response.text)
f.close()
