'use client'

import Image from 'next/image'
import Link from 'next/link'
import { useEffect, useState } from 'react'

import type { SocialNetworkName } from '../../types/auth.types'
import { getSocialNetworkAuthLink } from '@/shared/api'

import classes from './SocialNetworksAuth.module.scss'

type SocialNetwork = {
  label: string
  value: SocialNetworkName
  iconPath: string
}

export const SocialNetworksAuth = () => {
  const [authLinks, setAuthLinks] = useState<Record<SocialNetworkName, string | null>>({
    telegram: null,
    vk: null,
    google: null,
    steam: null,
    yandex: null
  })

  useEffect(() => {
    const loadAuthLinks = async () => {
      const socialNetworksNames = Object.keys(authLinks) as SocialNetworkName[]

      const links = await Promise.all(
        socialNetworksNames.map(async (socialNetworkName) => {
          return [socialNetworkName, await getSocialNetworkAuthLink(socialNetworkName)]
        })
      )

      setAuthLinks(Object.fromEntries(links))
    }

    loadAuthLinks()
  }, [])

  const socialNetworks: SocialNetwork[] = [
    {
      label: 'Телеграм',
      value: 'telegram',
      iconPath: '/social/white/telegram.svg'
    },
    {
      label: 'Вконтакте',
      value: 'vk',
      iconPath: '/social/white/vk.svg'
    }
    // {
    //   label: 'Яндекс',
    //   value: 'yandex',
    //   iconPath: '/social/white/yandex.svg'
    // },
    // {
    //   label: 'Steam',
    //   value: 'steam',
    //   iconPath: '/social/white/steam.svg'
    // },
    // {
    //   label: 'Google',
    //   value: 'google',
    //   iconPath: '/social/white/google.svg'
    // }
  ]

  return (
    <ul className={classes.socialNetworksAuth}>
      {socialNetworks.map(({ label, value, iconPath }) => {
        const link = authLinks[value]

        if (!link) {
          return (
            <li key={label}>
              <button type='button'>
                <Image src={iconPath} width={49} height={49} alt={label} />
              </button>
            </li>
          )
        }

        return (
          <li key={label}>
            <Link href={link}>
              <Image src={iconPath} width={49} height={49} alt={label} />
            </Link>
          </li>
        )
      })}
    </ul>
  )
}
