'use client'

import Image from 'next/image'
import Link from 'next/link'
import Popup from 'reactjs-popup'
import { useState } from 'react'
import cn from 'classnames'

import { Button, NavPopup, PriceWithCurrency } from '@/shared/ui'

import { useAuthModal } from '@/shared/hooks'
import { useUser } from '@/shared/hooks'
import { useAuth } from '@/shared/hooks'

import classes from './UserEntry.module.scss'

export const UserEntry = () => {
  const { isAuth, logout } = useAuth()
  const { user } = useUser()
  const { openAuthModal } = useAuthModal()

  const [isUserPopupOpen, setUserPopupOpen] = useState(false)

  if (!user || !isAuth) {
    return (
      <div className={classes.userEntry}>
        <Button className={classes.authBtn} boxShadow onClick={openAuthModal}>
          <span>
            <Image src='/icons/fingerprint.svg' width={31} height={31} alt='Отпечаток пальца' />
          </span>
          Войти
        </Button>
      </div>
    )
  }

  return (
    <div className={classes.userEntry}>
      <Popup
        className={classes.userPopup}
        open={isUserPopupOpen}
        arrow={false}
        closeOnDocumentClick
        position='bottom center'
        overlayStyle={{
          display: 'none'
        }}
        onClose={() => {
          setUserPopupOpen(false)
        }}
        trigger={
          <div
            className={classes.user}
            onClick={() => {
              setUserPopupOpen(true)
            }}
          >
            <Link
              href='/deposit'
              className={classes.balanceUpLink}
              onClick={(e) => {
                e.stopPropagation()
              }}
            >
              <Image src='/icons/plus.svg' width={15} height={15} alt='Плюс' />
            </Link>

            <PriceWithCurrency
              className={classes.balance}
              image={{
                width: 28,
                height: 27
              }}
            >
              {new Intl.NumberFormat('de-DE').format(user.currentBalance)}
            </PriceWithCurrency>

            <div className={classes.avatar}>
              <Image src='/placeholders/avatar.png' width={55} height={55} alt='Аватарка' />
            </div>
          </div>
        }
      >
        {/* @ts-expect-error: Skip lib type */}
        {(close) => {
          return <NavPopup className={classes.navPopup} onClose={close} onLogout={logout} />
        }}
      </Popup>

      <Link
        href='/deposit'
        className={cn(classes.balanceUpLink, classes.separated)}
        onClick={(e) => {
          e.stopPropagation()
        }}
      >
        <Image src='/icons/plus.svg' width={15} height={15} alt='Плюс' />
      </Link>

      <Link className={cn(classes.user, classes.asLink)} href='/profile'>
        <div className={classes.balanceUpLink}>
          <Image src='/icons/plus.svg' width={15} height={15} alt='Плюс' />
        </div>

        <PriceWithCurrency
          className={classes.balance}
          image={{
            width: 28,
            height: 27
          }}
        >
          {new Intl.NumberFormat('de-DE').format(user.currentBalance)}
        </PriceWithCurrency>

        <div className={classes.avatar}>
          <Image src='/placeholders/avatar.png' width={55} height={55} alt='Аватарка' />
        </div>
      </Link>
    </div>
  )
}
