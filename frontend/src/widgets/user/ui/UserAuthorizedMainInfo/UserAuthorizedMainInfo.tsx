'use client'

import { useState } from 'react'
import Image from 'next/image'
import cn from 'classnames'

import { UserAvatarUpload, UserNameChange } from '@/features/user'
import { IdCopier } from '@/shared/ui'
import { UserBalanceInfo } from '@/entities/user'
import { BestDroppedItem } from '@/entities/loot'
import { FavoriteCase } from '@/entities/cases'

import { useUser } from '@/shared/hooks'

import classes from './UserAuthorizedMainInfo.module.scss'

export const UserAuthorizedMainInfo = () => {
  const { user } = useUser()
  const [isUserNameChangeInProcess, setUserNameChangeInProcess] = useState(false)

  return (
    <div className={classes.userAuthorizedMainInfo}>
      <div className={classes.avatar}>
        <Image src='/placeholders/avatar-big.png' width={140} height={140} alt='Аватарка' />

        <div className={classes.avatarUploadBtn}>
          <UserAvatarUpload />
        </div>
      </div>

      <div className={classes.userInfo}>
        <div className={classes.topLine}>
          {!isUserNameChangeInProcess ? (
            <div className={classes.userNameContainer}>
              <p className={classes.userName}>{user?.userName}</p>

              <button
                className={classes.userNameChangeBtn}
                type='button'
                onClick={() => {
                  setUserNameChangeInProcess(true)
                }}
              >
                Изменить
              </button>
            </div>
          ) : (
            <UserNameChange
              userName={user?.userName || ''}
              onUserNameSubmit={() => {
                setUserNameChangeInProcess(false)
              }}
            />
          )}

          {user ? <IdCopier className={classes.idCopier} id={user.id} /> : null}
        </div>

        <div className={classes.balance}>
          <UserBalanceInfo />
        </div>
      </div>

      <div className={cn(classes.balance, classes.mobile)}>
        <UserBalanceInfo />
      </div>

      <div className={classes.bestDrop}>
        <BestDroppedItem userId={user?.id || ''} />
      </div>

      <div className={classes.favoriteCase}>
        <FavoriteCase userId={user?.id || ''} />
      </div>
    </div>
  )
}
