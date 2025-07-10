'use client'

import Image from 'next/image'
import Link from 'next/link'
import { useEffect, useState } from 'react'

import { useAuth } from '@/shared/hooks'

import classes from './SocialNetworksAuth.module.scss'

export const SocialNetworksAuth = () => {
  const [vkLink, setVkLink] = useState('')

  const { getTelegramAuthLink, getVkAuthLink } = useAuth()

  const getVkLink = async () => {
    const vkLink = await getVkAuthLink()
    setVkLink(vkLink)
  }

  useEffect(() => {
    getVkLink()
  }, [])

  const socialNetworks = [
    {
      label: 'Телеграм',
      value: 'tg',
      iconPath: '/social/white/telegram.svg'
    },
    {
      label: 'Вконтакте',
      value: 'vk',
      iconPath: '/social/white/vk.svg'
    },
    {
      label: 'Яндекс',
      value: 'yandex',
      iconPath: '/social/white/yandex.svg'
    },
    {
      label: 'Steam',
      value: 'steam',
      iconPath: '/social/white/steam.svg'
    },
    {
      label: 'Google',
      value: 'google',
      iconPath: '/social/white/google.svg'
    }
  ]

  return (
    <ul className={classes.socialNetworksAuth}>
      {socialNetworks.map(({ label, value, iconPath }) => {
        if (value === 'tg') {
          return (
            <li key={label}>
              <Link href={getTelegramAuthLink()}>
                <Image src={iconPath} width={49} height={49} alt={label} />
              </Link>
            </li>
          )
        }

        if (value === 'vk') {
          return (
            <li key={label}>
              <Link href={vkLink}>
                <Image src={iconPath} width={49} height={49} alt={label} />
              </Link>
            </li>
          )
        }

        return (
          <li key={label}>
            <button type='button'>
              <Image src={iconPath} width={49} height={49} alt={label} />
            </button>
          </li>
        )
      })}
    </ul>
  )
}
