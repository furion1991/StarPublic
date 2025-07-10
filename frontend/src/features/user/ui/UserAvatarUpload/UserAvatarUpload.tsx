import Image from 'next/image'

import classes from './UserAvatarUpload.module.scss'

export const UserAvatarUpload = () => {
  return (
    <div className={classes.userAvatarUpload}>
      <button type='button'>
        <Image src='/icons/upload.svg' width={15} height={16} alt='Загрузка' /> Загрузить
      </button>
    </div>
  )
}
