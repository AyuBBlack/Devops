Для начала я создал 2 раздела на одном диске.

Первый раздел для ext4, xfs, а второй для swap.

`fdisk /dev/vdb` 

Получил: 
```
lsblk
NAME   MAJ:MIN RM SIZE RO TYPE MOUNTPOINT
vda    252:0    0  15G  0 disk
├─vda1 252:1    0   1M  0 part
└─vda2 252:2    0  15G  0 part /
vdb    252:16   0  20G  0 disk
├─vdb1 252:17   0  10G  0 part
└─vdb2 252:18   0  10G  0 part
```
Далее делаю инициализаю диска

`pvcreate /dev/vdb`

Создаю группу для первого раздела

vgcreate volume /dev/vdb1 

Делаю в разделе vdb1 файловые системы ext4 и xfs

Предварительно установив xfsprogs

Создаю 2 lvm в группе **volume** по 1 GB назвав их **vol_1 vol_2**

`lvcreate -L 1G -n vol_1 volume`
`lvcreate -L 1G -n vol_2 volume`

Делаю файловую систему для **vol_1 и vol_2** 

```
mkfs.ext4 /dev/volume/vol_1
mkfs.xfs /dev/volume/vol_2
``` 

То же самое делаю для swap. 

Однако как делать свап я уже посмотрел на сайте: https://www.dmosk.ru/instruktions.php?object=lvm

Собственно у меня получилась вот такая последовательность

`mkswap /dev/vol_swap/swap_1`

После чего попробовал смонтировать это всё.

Для монтирования xfs пришлось создать начала директорию

`mkdir -p /var/lib/db`

lvdisplay посмотрел информацию для точности.

```
mount /dev/volume/vol_1 /var/log
mount /dev/volume/vol_2 /var/lib/db
```
Получил:
```
/dev/mapper/volume-vol_1  976M  2.6M  907M   1% /var/log
/dev/mapper/volume-vol_2 1014M   40M  975M   4% /var/lib/db
```


Теперь swap

`swapon /dev/vol_swap/swap_1`

swapon -s 

```
Filename                                Type            Size    Used    Priority
/dev/dm-2                               partition       1048572 0       -2
root@cloud-1:/home/ubuntu# swapon -L -s
```


```
vdb                 252:16   0  20G  0 disk
├─vdb1              252:17   0  10G  0 part
│ ├─volume-vol_1    253:0    0   1G  0 lvm  /var/log
│ └─volume-vol_2    253:1    0   1G  0 lvm  /var/lib/db
└─vdb2              252:18   0  10G  0 part
  └─vol_swap-swap_1 253:2    0   1G  0 lvm  [SWAP]
```
Командой `blkid` получаю все UUID

```
/dev/vda2: UUID="82afb880-9c95-44d6-8df9-84129f3f2cd1" TYPE="ext4" PARTUUID="5aa474f9-767f-4ae4-bf42-d1b1ca657053"
/dev/vda1: PARTUUID="ec0944f8-90a5-4e74-9453-d4d8d03bd53d"
/dev/vdb1: UUID="WFmy9g-bqEj-2Y1d-M2Q4-ZOyI-GXdF-a4c458" TYPE="LVM2_member" PARTUUID="efb78c4e-3b3d-c04b-adba-963bb6a5a713"
/dev/vdb2: UUID="CtSkOh-0IL8-ckwV-Rwqm-wif7-Szgl-ntUtnZ" TYPE="LVM2_member" PARTUUID="0ae954c6-c00a-924c-9428-d4cd5587ffc9"
/dev/mapper/volume-vol_1: UUID="3769d170-2c9b-4034-97a0-948a39bd828b" TYPE="ext4"
/dev/mapper/volume-vol_2: UUID="4d7ecc75-3332-448b-86db-5eab1875f870" TYPE="xfs"
/dev/mapper/vol_swap-swap_1: UUID="216b879a-af96-4cb0-a04d-5a5756261f60" TYPE="swap"
"
```
Заполнил fstab и как итог:

```
# /etc/fstab: static file system information.
#
# Use 'blkid' to print the universally unique identifier for a
# device; this may be used with UUID= as a more robust way to name devices
# that works even if disks are added and removed. See fstab(5).
#
# <file system> <mount point>   <type>  <options>       <dump>  <pass>
# / was on /dev/vda2 during installation
UUID=82afb880-9c95-44d6-8df9-84129f3f2cd1 /               ext4    errors=remount-ro 0       1

UUID=3769d170-2c9b-4034-97a0-948a39bd828b /var/log        ext4    defaults   0   1
UUID=4d7ecc75-3332-448b-86db-5eab1875f870 /var/lib/db      xfs    defaults   0   1
UUID=216b879a-af96-4cb0-a04d-5a5756261f60 swap            swap    defaults   0   0
```
