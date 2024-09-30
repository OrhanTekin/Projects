import tkinter as tk
import customtkinter as ctk
import yt_dlp
import os
from PIL import Image

#show progress in app
def my_hook(d):
    if d['status'] == 'downloading':
        curr_percentage = str(int(d['downloaded_bytes'] / d['total_bytes'] * 100))
        progressPer.configure(text=curr_percentage + '%')
        progressPer.update()

        progressBar.set(float(curr_percentage)/100)

#download method -> 2 downloads 1 for audio 1 for video
def download_video():
    if not directory_entry.get():
        update_directory_entry(f"{os.environ['USERPROFILE']}/Downloads/YouTubeVideos")

    ydl_opts = {
        'format': f'bestvideo[height<={resolution_combobox.get()}]+bestaudio',
        'outtmpl': f'{directory_entry.get()}/%(title)s.%(ext)s',
        #'external_downloader': 'aria2c',  # Using aria2c for faster downloads -> need to fix progress bar
        #'external_downloader_args': ['-x', '16', '-k', '1M'],
        'progress_hooks': [my_hook],
        'writethumbnail': True,
        'postprocessors': [{
            'format': 'jpg',
            'key': 'FFmpegThumbnailsConvertor',
            'when': 'before_dl'
        },
        {
            'actions': [(yt_dlp.postprocessor.metadataparser.MetadataParserPP.replacer,
                         'title',
                         '[\U0000002A\U0000005C\U0000002F\U0000003A\U00000022\U0000003F\U0000007C\U00010000-\U0010FFFF]',
                         '')],
            'key': 'MetadataParser',
            'when': 'pre_process'
        }],
    }

    try:
        url = link_textfield.get()
        with yt_dlp.YoutubeDL(ydl_opts) as ydl:
            video_info = ydl.extract_info(url, download=False)
            video_title = video_info.get('title', None)
            title.configure(text=video_title, text_color="white", font=("Comic Sans MS",20,'bold'))

            finishLabel.configure(text="")
            ydl.download(url)
            finishLabel.configure(text="Downloaded!", text_color="white")

            path_to_thumbnail = f'{directory_entry.get()}/{video_title}.jpg'
            thumbnail_image = Image.open(path_to_thumbnail)
            thumbnail = ctk.CTkImage(light_image=thumbnail_image, dark_image=thumbnail_image, size=(140, 140))
            thumbnail_label.configure(image=thumbnail)
    except Exception as e:
        print(e)
        finishLabel.configure(text="The video could not be downloaded!", text_color="red")


#open file explorer dialog
def open_directory():
    # Show the open file dialog by specifying path
    file_path = tk.filedialog.askdirectory(initialdir=f"{os.environ['USERPROFILE']}/Downloads/YouTubeVideos")
    if not file_path:
        file_path = directory_entry.get()
    update_directory_entry(file_path)

#update visible directory path
def update_directory_entry(file_path):
    directory_entry.configure(state=tk.NORMAL)
    directory_entry.delete(0, tk.END)
    directory_entry.insert(0, file_path)
    directory_entry.configure(state=tk.DISABLED)

#system settings
ctk.set_appearance_mode("System")
ctk.set_default_color_theme("dark-blue")

#app frame
app = ctk.CTk()
app.geometry("720x550")
app.maxsize(720,550)
app.title("YouTube Downloader")

#adding frames to order
upper_frame = ctk.CTkFrame(app, width=550,height=350,fg_color='transparent', border_width=2, border_color='white',
                                     corner_radius=10,bg_color='#181818')
upper_frame.place(relx=0.5,rely=0.35,anchor=tk.CENTER)

bottom_frame = ctk.CTkFrame(app, width=400,height=100, fg_color='transparent',border_width=2, border_color='white',
                                      corner_radius=10,bg_color='#181818')
bottom_frame.place(relx=0.3,rely=0.8,anchor=tk.CENTER)

thumbnail_frame = ctk.CTkFrame(upper_frame, width=150,height=150,fg_color='transparent', border_width=5,
                                         border_color='darkred',bg_color='#181818')
thumbnail_frame.place(relx=0.84,rely=0.25,anchor=tk.CENTER)

## Adding UI elements

#title label
title = ctk.CTkLabel(upper_frame, text="Insert YouTube link", wraplength=400)
title.configure(font=("Comic Sans MS",35,'bold'))
title.place(relx=0.33,rely=0.2, anchor=tk.CENTER)

#link input
link_textfield = ctk.CTkEntry(upper_frame, width=280, height=35,placeholder_text='https://www.youtube.com/watch?v=',
                                        border_color='white', fg_color='#181818')
link_textfield.place(relx=0.28,rely=0.415,anchor=tk.CENTER)

#choose desired resolution
all_resolutions = ["240", "360", "480", "720", "1080", "1440", "2160", "4320"]
resolution_combobox = ctk.CTkComboBox(upper_frame, width=75,height=35,values=all_resolutions,dropdown_font=("Comic Sans MS",15),
                                                button_hover_color='darkred',border_color='white', fg_color='#181818',
                                                button_color='white',dropdown_hover_color='#181818')
resolution_combobox.place(relx=0.61,rely=0.415,anchor=tk.CENTER)

#finished download
finishLabel = ctk.CTkLabel(upper_frame,text="")
finishLabel.place(relx=0.5,rely=0.55,anchor=tk.CENTER)
finishLabel.configure(font=("Comic Sans MS",15,'bold'))

#progress percentage
progressPer = ctk.CTkLabel(upper_frame, text="0%", text_color='white')
progressPer.place(relx=0.5, rely=0.65,anchor=tk.CENTER)

#progress bar
progressBar = ctk.CTkProgressBar(upper_frame, width=400, progress_color='red')
progressBar.set(0)
progressBar.place(relx=0.5,rely=0.7,anchor=tk.CENTER)

#download button
download = ctk.CTkButton(upper_frame, text="Download", width=200, height=70, hover_color='darkred',
                                command = download_video, fg_color='transparent', border_width=2,border_color='white',corner_radius=10)
download.configure(font=("Comic Sans MS",30,'bold'))
download.place(relx=0.5,rely=0.85,anchor=tk.CENTER)

#download path label
download_label = ctk.CTkLabel(bottom_frame, text="Download Folder")
download_label.configure(font=("Comic Sans MS",20,'bold'))
download_label.place(relx=0.5,rely=0.2,anchor=tk.E)

#browse entry for directory
directory_entry = ctk.CTkEntry(bottom_frame, width=300, height=30, border_color='white', fg_color='#181818')
directory_entry.place(relx=0.4,rely=0.6,anchor=tk.CENTER)
update_directory_entry(f"{os.environ['USERPROFILE']}/Downloads/YouTubeVideos")

#open directory button
directory = ctk.CTkButton(bottom_frame, text="Browse",width=50,height=30,command = open_directory,
                                    fg_color='transparent',border_color='white',hover_color='darkred',border_width=2)
directory.place(relx=0.85,rely=0.6,anchor=tk.CENTER)

placeholder_thumbnail_image = Image.open('./placeholder_thumbnail.jpg')
placeholder_thumbnail = ctk.CTkImage(light_image=placeholder_thumbnail_image, dark_image=placeholder_thumbnail_image, size=(140, 140))
thumbnail_label = ctk.CTkLabel(thumbnail_frame, image=placeholder_thumbnail)
thumbnail_label.place(relx=0.5, rely=0.5,anchor=tk.CENTER)

#Run app
app.mainloop()
