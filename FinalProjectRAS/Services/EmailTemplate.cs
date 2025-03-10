using FinalProjectRAS.ViewModels;

namespace FinalProjectRAS.Services
{
    public class EmailTemplate
    {
        public static string BuildEmailBody(DataEmailVM data)
        {
            return $@"
                <html>
                    <head>
                        <style>
                            .email-container {{
                                font-family: Arial, sans-serif;
                                padding: 20px;
                                background-color: #f4f4f4;
                            }}
                            .email-header {{
                                text-align: center;
                                font-size: 24px;
                            }}
                            .email-body {{
                                font-size: 16px;
                            }}
                            .btn {{
                                display: inline-block;
                                padding: 12px 24px;
                                background-color: #1E5ECE;
                                color: white;
                                text-decoration: none;
                                border-radius: 5px;
                            }}
                            .footer {{
                                font-size: 12px;
                                text-align: center;
                                margin-top: 30px;
                            }}
                            .deadline {{
                                color: red;
                            }}
                            .pt-20 {{
                                padding-top: 20px;
                            }}
                            .pb-20 {{
                                padding-bottom: 20px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <div class='email-header'>
                                <img src='cid:logo' alt='Company Logo' width='450'/>
                                <h2>Undangan Technical Test (Online)</h2>
                            </div>
                            <div class='email-body'>
                                <p>Dear Candidates,</p>
                                <p class='pb-20'>Terima kasih telah bersedia mengikuti rangkaian proses rekrutmen PT Berca Hardayaperkasa. Pada tahap ini, anda diminta untuk mengikuti proses technical test secara online. Berikut merupakan Email dan Password yang digunakan untuk login ke website test online :</p>
                                <p>Email : {data.Email}</p>
                                <p>Password : {data.Password}</p>
                                <p class='pt-20'>Klik tombol dibawah untuk masuk ke website technical test</p>
                                <a href='{data.Url}'><p class='btn'>Masuk</p></a>
                                <p>Jika tombol diatas tidak berfungsi, copy and paste link dibawah ini ke browser anda:</p>
                                <p><a href='{data.Url}'>{data.Url}</a></p>
                                </br>
                                <p><b>NOTE : </b> pastikan mengerjakan technical test ini sebelum <span class='deadline'><b>{data.Deadline}</b></span> atau link akan <b>Expired</b></p>
                            </div>
                            <div class='footer'>
                                <p>TECHNICAL TEST ONLINE INI SIFATNYA CONFIDENTIAL, MOHON UNTUK TIDAK SEBAR LUASKAN!!!</p>
                                <p>HATI-HATI PENIPUAN!!</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }
    }
}

