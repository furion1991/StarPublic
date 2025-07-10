'use client'

import cn from 'classnames'
import Link from 'next/link'
import { useEffect, useState } from 'react'

import { Button } from '@/shared/ui'

import { getSocialNetworkAuthLink } from '@/shared/api'
import { useUser } from '@/shared/hooks'

import classes from './ClaimSubscriptionBonusButton.module.scss'

type NetworkName = 'vk' | 'telegram'

type NetworkSubscriptions = Record<NetworkName, boolean>

type ClaimSubscriptionBonusButtonProps = {
  networkName: NetworkName
}

export const ClaimSubscriptionBonusButton = ({
  networkName
}: ClaimSubscriptionBonusButtonProps) => {
  const { user } = useUser()

  const [authLink, setAuthLink] = useState<string | null>()

  const networksSubscriptions: NetworkSubscriptions = {
    vk: Boolean(user?.isSubscribedToVk),
    telegram: Boolean(user?.isSubscribedToTg)
  }

  useEffect(() => {
    const loadAuthLink = async () => {
      const link = await getSocialNetworkAuthLink(networkName, 'bonuses')

      setAuthLink(link)
    }

    loadAuthLink()
  }, [])

  const isSubscribed = networksSubscriptions[networkName]

  return (
    <Link
      href={authLink || '/'}
      className={cn(classes.link, {
        [classes.linkDisabled]: isSubscribed
      })}
    >
      <Button
        className={cn({
          [classes.filled]: isSubscribed
        })}
      >
        {!isSubscribed ? 'Я подписался' : 'Выполнено'}
      </Button>
    </Link>
  )
}
