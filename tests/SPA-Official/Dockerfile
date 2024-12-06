FROM scottyhardy/docker-wine

WORKDIR /APP

# exe files
# kurs - https://cez2.wi.pb.edu.pl/moodle/course/view.php?id=1094
# direct - https://cez2.wi.pb.edu.pl/moodle/mod/resource/view.php?id=47469

COPY ./SPA-Official.exe ./SPA-Official.exe

CMD [ "wine", "SPA-Official.exe", "/app/code" ]