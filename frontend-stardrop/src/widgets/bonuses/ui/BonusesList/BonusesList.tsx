'use client'

import Image from 'next/image'
import { useEffect, useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useSearchParams, useRouter, usePathname } from 'next/navigation'

import { DailyBonusModal, GiveawayBonusModal, OneTimeBonusModal } from '@/entities/bonuses'
import {
  ClaimDailyBonusButton,
  ClaimSubscriptionBonusButton,
  GiveawaySubscribeButton
} from '@/features/bonuses'
import { Button } from '@/shared/ui'

import { claimBonusForSubscription } from '../../api/bonuses'

import classes from './BonusesList.module.scss'

type BonusName = 'daily' | 'one-time' | 'giveaway'

type Bonus = {
  value: BonusName
  label: string
  text: string
  img: {
    src: string
    width: number
    height: number
  }
}

export const BonusesList = () => {
  const queryClient = useQueryClient()
  const searchParams = useSearchParams()
  const pathname = usePathname()
  const router = useRouter()

  const [openedBonusModal, setOpenedBonusModal] = useState<BonusName>()

  const { mutate: claimBonus } = useMutation({
    mutationFn: claimBonusForSubscription,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['me']
      })
    },
    onSettled: () => {
      setOpenedBonusModal('one-time')

      if (pathname) {
        router.replace(pathname)
      }

      localStorage.removeItem('codeVerifier')
      localStorage.removeItem('state')
    }
  })

  const bonuses: Bonus[] = [
    {
      value: 'daily',
      label: 'Ежедневный бонус',
      text: 'Ежедневный бонус от 1 до 2500 рублей каждые 24 часа',
      img: {
        src: '/img/bonus-time.png',
        width: 136,
        height: 131
      }
    },
    {
      value: 'one-time',
      label: 'Разовый бонус',
      text: 'Разовый бонус дается за выполнение списка заданий',
      img: {
        src: '/img/bonus-quest.png',
        width: 160,
        height: 149
      }
    },
    {
      value: 'giveaway',
      label: 'Бонус «розыгрыш»',
      text: 'Розыгрыш от 100 до 500 бонусов',
      img: {
        src: '/img/bonus-chest.png',
        width: 129,
        height: 146
      }
    }
  ]

  useEffect(() => {
    const hash = window.location.hash
    const isTelegramHashExist = window.location.hash.startsWith('#tgAuthResult=')

    const state = localStorage.getItem('state')
    const deviceId = searchParams?.get('device_id')
    const codeVerifier = localStorage.getItem('codeVerifier')
    const code = searchParams?.get('code')

    // tg
    if (isTelegramHashExist) {
      const authHashValue = hash.split('=')[1]

      claimBonus({
        provider: 'tg',
        data: authHashValue
      })
    }

    // vk
    if (state && deviceId && codeVerifier && code) {
      claimBonus({
        provider: 'vk',
        vkData: {
          state,
          deviceId,
          codeVerifier,
          code
        }
      })
    }
  }, [])

  const closeBonusModal = () => {
    setOpenedBonusModal(undefined)
  }

  return (
    <>
      <ul className={classes.bonusesList}>
        {bonuses.map(({ value, label, text, img }) => {
          return (
            <li key={label}>
              <h4>{label}</h4>

              <p>{text}</p>

              <Button
                onClick={() => {
                  setOpenedBonusModal(value)
                }}
              >
                Открыть ›
              </Button>

              <Image
                className={classes.bonusImg}
                src={img.src}
                width={img.width}
                height={img.height}
                alt={label}
              />
            </li>
          )
        })}
      </ul>

      <DailyBonusModal
        open={openedBonusModal === 'daily'}
        onClose={closeBonusModal}
        ClaimButtonSlot={<ClaimDailyBonusButton />}
      />

      <OneTimeBonusModal
        open={openedBonusModal === 'one-time'}
        onClose={closeBonusModal}
        SocialSlots={{
          vk: <ClaimSubscriptionBonusButton networkName='vk' />,
          tg: <ClaimSubscriptionBonusButton networkName='telegram' />
        }}
      />

      <GiveawayBonusModal
        open={openedBonusModal === 'giveaway'}
        onClose={closeBonusModal}
        GiveawaySubscribeButtonSlot={<GiveawaySubscribeButton />}
      />
    </>
  )
}
